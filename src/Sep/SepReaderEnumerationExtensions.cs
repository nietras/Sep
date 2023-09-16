//#define SEPTRACEPARALLEL
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
        //return ParallelEnumerateInternalOneWorkItemPerRow(reader, select, maxDegreeOfParallelism);
    }

    static IEnumerable<T> ParallelEnumerateInternal<T>(this SepReader reader, SepReader.RowFunc<T> select,
        int maxDegreeOfParallelism, int maxRowsInPlay = 16 * 1024)
    {
        var rowStates = new List<SepReaderRowStateForParallel<T>>(maxDegreeOfParallelism);

        var workers = new List<LoopThreadPoolWorkItem<T>>(maxDegreeOfParallelism);
        var readyWorkers = new ConcurrentBag<LoopThreadPoolWorkItem<T>>();

        var rowStatesUnused = new Stack<int>(maxDegreeOfParallelism);
        var rowStatesExecutingOrdered = new Queue<int>(maxDegreeOfParallelism);
        var rowStatesForExecuting = new ConcurrentQueue<SepReaderRowStateForParallel<T>>();

        using var someWorkItemsDoneEvent = new ManualResetEvent(false);
        Action<int> enqueueDone = workIndex => { someWorkItemsDoneEvent.Set(); };

        try
        {
            var rowsToReadPerLoop = Math.Min(maxDegreeOfParallelism, maxRowsInPlay);
            var readerNext = true;
            while (readerNext || rowStatesExecutingOrdered.Count > 0)
            {
                //workers.Count < maxDegreeOfParallelism ||
                var rowsRead = 0;
                while ((rowsRead < rowsToReadPerLoop && rowStatesExecutingOrdered.Count < maxRowsInPlay)
                       && (readerNext = reader.MoveNext()))
                {
                    SepReaderRowStateForParallel<T> rowState;
                    if (rowStatesUnused.TryPop(out var rowStatesIndex))
                    {
                        rowState = rowStates[rowStatesIndex];
                    }
                    else
                    {
                        rowStatesIndex = rowStates.Count;
                        rowState = new SepReaderRowStateForParallel<T>(reader, rowStatesIndex, select);
                        rowStates.Add(rowState);
                    }

                    rowState.IsDone = false;
                    reader.CopyNewRowTo(rowState);

                    rowStatesExecutingOrdered.Enqueue(rowStatesIndex);
                    rowStatesForExecuting.Enqueue(rowState);

                    ++rowsRead;
                }

                // We want to end up in situation where there are never no workers ready to execute,
                // since they should be constantly occupied i.e. by feeding new rows to them.
                var readyWorkersCount = readyWorkers.Count;
                if (readyWorkersCount > 0)
                {
                    if (rowsToReadPerLoop < maxRowsInPlay)
                    {
                        rowsToReadPerLoop = Math.Min((rowsToReadPerLoop * 2), maxRowsInPlay);
#if SEPTRACEPARALLEL
                        Trace.WriteLine($"Increased rows to read {rowsToReadPerLoop}");
#endif
                    }
                    // Only queue worker on there are no workers running,
                    // and we have not reached maxRowsInPlay being executed
                    if (workers.Count == readyWorkers.Count ||
                        rowStatesExecutingOrdered.Count >= maxRowsInPlay)
                    {
                        if (readyWorkers.TryTake(out var worker))
                        {
#if SEPTRACEPARALLEL
                            Trace.WriteLine("Queue ready worker");
#endif
                            ThreadPool.UnsafeQueueUserWorkItem(worker, preferLocal: false);
                        }
                        else
                        {
                            Debug.Assert(false, "There should always be a ready worker here");
                        }
                    }
                }
                else if (rowStatesForExecuting.Count > workers.Count && rowStates.Count > workers.Count
                         && workers.Count < maxDegreeOfParallelism)
                {
#if SEPTRACEPARALLEL
                    Trace.WriteLine($"Add worker {nameof(rowStatesForExecuting)}:{rowStatesForExecuting.Count} {nameof(workers)}:{workers.Count}  {nameof(rowStates)}:{rowStates.Count}");
#endif
                    var worker = new LoopThreadPoolWorkItem<T>(rowStatesForExecuting, readyWorkers, enqueueDone);
                    workers.Add(worker);
                    ThreadPool.UnsafeQueueUserWorkItem(worker, preferLocal: false);
                }

                var returnCount = 0;
                SepReaderRowStateForParallel<T> nextRowStateToReturn;
                while (rowStatesExecutingOrdered.TryPeek(out var nextRowStateIndexToReturn) &&
                       (nextRowStateToReturn = rowStates[nextRowStateIndexToReturn]).IsDone)
                {
                    var index = rowStatesExecutingOrdered.Dequeue();
                    Debug.Assert(nextRowStateIndexToReturn == index);
                    Debug.Assert(nextRowStateIndexToReturn == nextRowStateToReturn.Index);
                    yield return nextRowStateToReturn.Result;
                    rowStatesUnused.Push(nextRowStateIndexToReturn);
                    ++returnCount;
                }

                if (rowStates.Count >= maxRowsInPlay)
                {
                    someWorkItemsDoneEvent.WaitOne(100);
                }
            }
        }
        finally
        {
            foreach (var rowState in rowStates)
            {
                rowState.Dispose();
            }
        }
    }

    static IEnumerable<T> ParallelEnumerateInternalOneWorkItemPerRow<T>(this SepReader reader, SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
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

        var rowStatesIndicesForExecuting = new ConcurrentQueue<int>();

        try
        {
            for (var workIndex = 0; workIndex < workerCount; workIndex++)
            {
                var rowState = new SepReaderRowStateForParallel<T>(reader, workIndex, select);
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

                    ThreadPool.UnsafeQueueUserWorkItem(workItemReady, preferLocal: true);

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

    sealed class SepReaderRowStateForParallel<T> : SepReaderRowState
    {
        readonly SepReader.RowFunc<T> _select;

        internal SepReaderRowStateForParallel(SepReader reader, int index, SepReader.RowFunc<T> select)
            : base(reader)
        {
            Index = index;
            _select = select;
        }

        public int Index { get; }
        public volatile bool IsDone = false;
        public T Result { get; set; } = default!;

        internal void Execute()
        {
            Result = _select(new(this));
            IsDone = true;
        }
    }

    class LoopThreadPoolWorkItem<T> : IThreadPoolWorkItem
    {
        readonly ConcurrentQueue<SepReaderRowStateForParallel<T>> _rowStatesForExecuting;
        readonly ConcurrentBag<LoopThreadPoolWorkItem<T>> _doneWorkItems;
        readonly Action<int> _signal;

        public LoopThreadPoolWorkItem(
            ConcurrentQueue<SepReaderRowStateForParallel<T>> rowStatesForExecuting,
            ConcurrentBag<LoopThreadPoolWorkItem<T>> doneWorkItems,
            Action<int> signal)
        {
            _rowStatesForExecuting = rowStatesForExecuting;
            _doneWorkItems = doneWorkItems;
            _signal = signal;
        }

        public void Execute()
        {
            while (_rowStatesForExecuting.TryDequeue(out var rowState))
            {
                rowState.Execute();
                _signal(rowState.Index);
            }
            _doneWorkItems.Add(this);
        }
    }

    class RowThreadPoolWorkItem<T> : IThreadPoolWorkItem
    {
        readonly SepReader.RowFunc<T> _select;
        readonly Action<int> _signalDone;

        public RowThreadPoolWorkItem(
            SepReaderRowStateForParallel<T> rowState, SepReader.RowFunc<T> select,
            int workIndex, Action<int> signalDone)
        {
            RowState = rowState;
            _select = select;
            WorkIndex = workIndex;
            _signalDone = signalDone;
        }

        public SepReaderRowStateForParallel<T> RowState { get; }
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
