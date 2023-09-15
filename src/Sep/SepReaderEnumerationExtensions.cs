using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace nietras.SeparatedValues;

public static class SepReaderEnumerationExtensions
{
    public static IEnumerable<T> Enumerate<T>(this SepReader reader, SepReader.RowFunc<T> select)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);

        foreach (var row in reader)
        {
            yield return select(row);
        }
    }

    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
    {
        if (maxDegreeOfParallelism <= 1) { return Enumerate(reader, select); }
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);
        return ParallelEnumerateInternal(reader, select, maxDegreeOfParallelism);
    }

    static IEnumerable<T> ParallelEnumerateInternal<T>(this SepReader reader, SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
    {
        // For now limit parallelism to max processor count times 2
        var workerCount = Math.Min(maxDegreeOfParallelism, Environment.ProcessorCount) * 2;

        // https://github.com/dotnet/runtime/blob/9aeed3023cad3889c56ab7f8c2eba0955d336987/src/libraries/System.Private.CoreLib/src/System/Threading/ThreadPoolWorkQueue.cs#L1526-L1542
        //ExecutionContext? context = ExecutionContext.Capture();
        //object tpcallBack = (context == null || context.IsDefault) ?
        //    new QueueUserWorkItemCallbackDefaultContext<TState>(callBack!, state) :
        //    (object)new QueueUserWorkItemCallback<TState>(callBack!, state, context);
        //s_workQueue.Enqueue(tpcallBack, forceGlobal: !preferLocal);

        var workItems = new RowThreadPoolWorkItem<T>[workerCount];
        var workIndicesReady = new Stack<int>(workerCount);

        var workIndicesDone = new ConcurrentQueue<int>();
        using var workSemaphore = new SemaphoreSlim(workIndicesReady.Count);
        Action<int> enqueueDone = workIndex => { workIndicesDone.Enqueue(workIndex); workSemaphore.Release(); };

        for (var workIndex = 0; workIndex < workerCount; workIndex++)
        {
            var rowState = new SepReaderRowState();
            workItems[workIndex] = new RowThreadPoolWorkItem<T>(rowState, select, workIndex, enqueueDone);
            workIndicesReady.Push(workIndex);
        }

        foreach (var row in reader)
        {
            workSemaphore.Wait();
            var dequeued = workIndicesDone.TryDequeue(out var workIndex);
            Debug.Assert(dequeued);
            var workItem = workItems[workIndex];
            //row.CopyTo(workItem.RowState);

            yield return select(row);
        }

        //public static bool UnsafeQueueUserWorkItem(IThreadPoolWorkItem callBack, bool preferLocal)
    }

    class RowThreadPoolWorkItem<T> : IThreadPoolWorkItem
    {
        readonly SepReader.RowFunc<T> _select;
        readonly Action<int> _enqueueDone;

        public RowThreadPoolWorkItem(
            SepReaderRowState rowState, SepReader.RowFunc<T> select,
            int workIndex, Action<int> enqueueDone)
        {
            RowState = rowState;
            _select = select;
            WorkIndex = workIndex;
            _enqueueDone = enqueueDone;
        }

        public SepReaderRowState RowState { get; }
        public int WorkIndex { get; }
        public T Result { get; set; } = default!;

        public void Execute()
        {
            //Result = _select(RowState);
            _enqueueDone(WorkIndex);
        }
    }
}
