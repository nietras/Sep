using System;
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
        // For now limit parallelism to max processor count
        var workerCount = Math.Min(maxDegreeOfParallelism, Environment.ProcessorCount);

        // Consider whether to capture ExecutionContext see:
        // https://github.com/dotnet/runtime/blob/9aeed3023cad3889c56ab7f8c2eba0955d336987/src/libraries/System.Private.CoreLib/src/System/Threading/ThreadPoolWorkQueue.cs#L1526-L1542
        //ExecutionContext? context = ExecutionContext.Capture();
        //object tpcallBack = (context == null || context.IsDefault) ?
        //    new QueueUserWorkItemCallbackDefaultContext<TState>(callBack!, state) :
        //    (object)new QueueUserWorkItemCallback<TState>(callBack!, state, context);
        //s_workQueue.Enqueue(tpcallBack, forceGlobal: !preferLocal);

        var workItems = new RowThreadPoolWorkItem<T>[workerCount];
        var workIndicesReady = new Stack<int>(workerCount);
        var workIndicesExecutingOrdered = new Queue<int>(workerCount);

        using var someWorkItemsDoneEvent = new ManualResetEvent(false);
        Action<int> enqueueDone = workIndex => { someWorkItemsDoneEvent.Set(); };

        try
        {
            for (var workIndex = 0; workIndex < workerCount; workIndex++)
            {
                var rowState = new SepReaderRowState(reader);
                workItems[workIndex] = new RowThreadPoolWorkItem<T>(rowState, select, workIndex, enqueueDone);
                workIndicesReady.Push(workIndex);
            }

            var readerNext = true;

            while (readerNext || workIndicesExecutingOrdered.Count > 0)
            {
                while (workIndicesReady.Count > 0 && (readerNext = reader.MoveNext()))
                {
                    var workIndexReady = workIndicesReady.Pop();
                    var workItemReady = workItems[workIndexReady];
                    reader.CopyNewRowTo(workItemReady.RowState);
                    workItemReady.IsDone = false;

                    ThreadPool.UnsafeQueueUserWorkItem(workItemReady, preferLocal: false);

                    workIndicesExecutingOrdered.Enqueue(workIndexReady);
                }

                someWorkItemsDoneEvent.WaitOne();
                RowThreadPoolWorkItem<T> nextWorkItemToReturn = null!;
                if (workIndicesExecutingOrdered.TryPeek(out var nextWorkIndexToReturn) &&
                    (nextWorkItemToReturn = workItems[nextWorkIndexToReturn]).IsDone)
                {
                    workIndicesExecutingOrdered.Dequeue();
                    Debug.Assert(nextWorkIndexToReturn == nextWorkItemToReturn.WorkIndex);
                    yield return nextWorkItemToReturn.Result;
                    workIndicesReady.Push(nextWorkIndexToReturn);
                }
            }
        }
        finally
        {
            foreach (var workItem in workItems)
            {
                workItem.RowState.Dispose();
            }
        }
    }

    class RowThreadPoolWorkItem<T> : IThreadPoolWorkItem
    {
        readonly SepReader.RowFunc<T> _select;
        readonly Action<int> _signalDone;

        public RowThreadPoolWorkItem(
            SepReaderRowState rowState, SepReader.RowFunc<T> select,
            int workIndex, Action<int> signalDone)
        {
            RowState = rowState;
            _select = select;
            WorkIndex = workIndex;
            _signalDone = signalDone;
        }

        public SepReaderRowState RowState { get; }
        public int WorkIndex { get; }
        public T Result { get; set; } = default!;
        public volatile bool IsDone = false;

        public void Execute()
        {
            Result = _select(new(RowState));
            IsDone = true;
            _signalDone(WorkIndex);
        }
    }
}
