//#define SEPTRACEPARALLEL
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace nietras.SeparatedValues;

public static class SepReaderEnumerationExtensions
{
    static readonly Action<string> Log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

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
        if (!reader.HasRows) { return Array.Empty<T>(); }
        //return ParallelEnumerateInternalBusyLooping(reader, select, maxDegreeOfParallelism);
        return ParallelEnumerateInternalRowStatesGroupPerWorker(reader, select, maxDegreeOfParallelism);
        //return ParallelEnumerateInternalManyRowStates(reader, select, maxDegreeOfParallelism);
        //return ParallelEnumerateInternalOneWorkItemPerRow(reader, select, maxDegreeOfParallelism);
    }

    class KeepDequeueThreadPoolWorkItem<T> : IThreadPoolWorkItem
    {
        readonly ConcurrentQueue<SepReaderRowStateForParallel<T>> _rowStatesForExecuting;
        //readonly SemaphoreSlim _semaphoreSlim;
        readonly ConcurrentBag<KeepDequeueThreadPoolWorkItem<T>> _doneWorkItems;
        readonly Action<int> _signal;

        public KeepDequeueThreadPoolWorkItem(
            ConcurrentQueue<SepReaderRowStateForParallel<T>> rowStatesForExecuting,
            //SemaphoreSlim semaphoreSlim,
            ConcurrentBag<KeepDequeueThreadPoolWorkItem<T>> doneWorkItems,
            Action<int> signal)
        {
            _rowStatesForExecuting = rowStatesForExecuting;
            //_semaphoreSlim = semaphoreSlim;
            _doneWorkItems = doneWorkItems;
            _signal = signal;
        }

        public void Execute()
        {
            var count = 0;
            //while (_semaphoreSlim.Wait(10))
            //{
            //    if (_rowStatesForExecuting.TryDequeue(out var rowState))
            //    {
            //        rowState.Execute();
            //        _signal(rowState.Index);
            //        ++count;
            //    }
            //    else
            //    {
            //        Debug.Assert(false, "semaphore / queue inconsistency");
            //    }
            //}
            while (_rowStatesForExecuting.TryDequeue(out var rowState))
            {
                rowState.Execute();
                _signal(rowState.Index);
                ++count;
            }
            //Log?.Invoke($"TEST Worker done {count}");
            _doneWorkItems.Add(this);
        }
    }

    static IEnumerable<T> ParallelEnumerateInternalBusyLooping<T>(this SepReader reader,
        SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
    {
        if (!reader.HasRows) { yield break; }

        // For now limit parallelism to max processor count
        var workerCount = Math.Min(maxDegreeOfParallelism, Environment.ProcessorCount);
        var maxRowsCount = workerCount * 32;

        var workers = new List<KeepDequeueThreadPoolWorkItem<T>>(workerCount);
        //var workIndicesReady = new Stack<int>(workerCount);
        //var workIndicesExecutingOrdered = new Queue<int>(workerCount);
        var readyWorkers = new ConcurrentBag<KeepDequeueThreadPoolWorkItem<T>>();

        var rowStates = new SepReaderRowStateForParallel<T>[maxRowsCount];
        var rowStatesUnused = new Stack<int>(maxDegreeOfParallelism);
        var rowStatesExecutingOrdered = new Queue<int>(maxDegreeOfParallelism);
        var rowStatesForExecuting = new ConcurrentQueue<SepReaderRowStateForParallel<T>>();
        //using var rowStatesForExecutingSemaphore = new SemaphoreSlim(initialCount: 0);
        //var rowStatesIndicesForExecuting = new ConcurrentQueue<int>();

        using var someWorkItemsDoneEvent = new AutoResetEvent(false);
        Action<int> enqueueDone = workIndex => { someWorkItemsDoneEvent.Set(); };

        try
        {
            for (var rowStateIndex = 0; rowStateIndex < maxRowsCount; rowStateIndex++)
            {
                var rowState = new SepReaderRowStateForParallel<T>(reader, rowStateIndex, select);
                rowStates[rowStateIndex] = rowState;
                rowStatesUnused.Push(rowStateIndex);
            }

            var waitCount = 0;
            var queueCount = 0;

            var readerNext = true;
            while (readerNext || rowStatesExecutingOrdered.Count > 0)
            {
                //while (rowStatesForExecuting.Count > workerCount &&
                //       workers.Count > 0 && readyWorkers.Count < 1)
                ////while (rowStatesForExecuting.Count >= rowStates.Length &&
                ////   workers.Count >= maxDegreeOfParallelism && readyWorkers.Count < 1)
                //{
                //    //Log?.Invoke($"TEST Waiting {rowStatesExecutingOrdered.Count} {readyWorkers.Count}:{workers.Count}");
                //    someWorkItemsDoneEvent.WaitOne(1);
                //    ++waitCount;
                //}
                // Fill up row states
                while (rowStatesUnused.Count > 0 && readerNext && (readerNext = reader.MoveNext()))
                {
                    var rowStateIndex = rowStatesUnused.Pop();
                    var rowState = rowStates[rowStateIndex];
                    rowState.IsDone = false;
                    reader.CopyNewRowTo(rowState);
                    rowStatesExecutingOrdered.Enqueue(rowStateIndex);
                    rowStatesForExecuting.Enqueue(rowState);
                    //rowStatesForExecutingSemaphore.Release();
                }
                // Yield results
                SepReaderRowStateForParallel<T>? rowStateMaybeDone;
                while (rowStatesExecutingOrdered.TryPeek(out var rowStateIndexNext) &&
                       (rowStateMaybeDone = rowStates[rowStateIndexNext]).IsDone)
                {
                    rowStatesExecutingOrdered.Dequeue();
                    yield return rowStateMaybeDone.Result;
                    rowStatesUnused.Push(rowStateIndexNext);
                }
                // Add worker if ??
                if (rowStatesForExecuting.Count > ((rowStates.Length - workerCount) / 2) ||
                    readyWorkers.Count == workers.Count)
                {
                    if (!readyWorkers.TryTake(out var worker))
                    {
                        if (workers.Count < maxDegreeOfParallelism)
                        {
                            worker = new KeepDequeueThreadPoolWorkItem<T>(rowStatesForExecuting, /*rowStatesForExecutingSemaphore,*/ readyWorkers, enqueueDone);
                            workers.Add(worker);
                            //Log?.Invoke($"TEST Added worker {workers.Count}");
                        }
                    }
                    if (worker != null)
                    {
                        ThreadPool.UnsafeQueueUserWorkItem(worker, preferLocal: true);
                        ++queueCount;
                        //Log?.Invoke($"TEST Queued worker");
                    }
                }
            }

            Log?.Invoke($"Workers {workers.Count} ThreadPoolQueue {queueCount} Wait {waitCount} Rows {reader._rowIndex}");
        }
        finally
        {
            foreach (var rowState in rowStates)
            {
                rowState.Dispose();
            }
        }
    }

    sealed class RowStatesThreadPoolWorkItem<T> : IThreadPoolWorkItem, IDisposable
    {
        readonly SepReaderRowState _protoRowState;
        readonly int _rowStateCountPerExecute;
        readonly SepReader.RowFunc<T> _select;
        readonly int _index;
        readonly Action<int> _signal;
        readonly List<SepReaderRowState> _rowStates;
        readonly List<T> _results;
        bool _disposedValue;

        public RowStatesThreadPoolWorkItem(
            SepReaderRowState protoRowState, int initialRowStateCount, SepReader.RowFunc<T> select,
            int index, Action<int> signal)
        {
            _protoRowState = protoRowState;
            _rowStateCountPerExecute = Math.Max(1, initialRowStateCount);
            _select = select;
            _index = index;
            _signal = signal;
            _rowStates = new(initialRowStateCount) { protoRowState };
            EnsureRowStates(initialRowStateCount);
            _results = new(initialRowStateCount);
        }

        public int RowsToExecute { get; set; }
        public List<SepReaderRowState> RowStates => _rowStates;
        public List<T> Results => _results;
        public volatile bool IsDone = false;

        public void Execute()
        {
#if SEPTRACEPARALLEL
            var b = Stopwatch.GetTimestamp();
#endif
            _results.Clear();
            Debug.Assert(RowsToExecute <= _rowStates.Count);
            for (var i = 0; i < RowsToExecute; i++)
            {
                var rowState = _rowStates[i];
                rowState.ResetSharedCache();
                _results.Add(_select(new(rowState)));
            }
            IsDone = true;
#if SEPTRACEPARALLEL
            var a = Stopwatch.GetTimestamp();
            var ms = ((a - b) * 1000.0) / Stopwatch.Frequency;
            Console.WriteLine($"{RowsToExecute,4} {ms,6:F3}");
#endif
            _signal(_index);
        }

        void EnsureRowStates(int maxRowsToExecute)
        {
            while (_rowStates.Count < maxRowsToExecute)
            {
                _rowStates.Add(_protoRowState.CloneWithSharedCache());
            }
        }

        void DisposeManaged()
        {
            foreach (var rowState in _rowStates)
            {
                rowState.Dispose();
            }
            _rowStates.Clear();
        }

        #region Dispose
        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    DisposeManaged();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }


    static IEnumerable<T> ParallelEnumerateInternalRowStatesGroupPerWorker<T>(this SepReader reader, SepReader.RowFunc<T> select,
        int maxDegreeOfParallelism, int maxRowsPerWorker = 32)
    {
        var readerHasMore = reader.MoveNext();
        if (!readerHasMore) { yield break; }

        //var enumerateMoreRows = MicrosecondsToTicks(100);
        //var oneRowStatePerWorkerThresholdMicroseconds = MicrosecondsToTicks(2000);
        static long MicrosecondsToTicks(int microseconds) => (microseconds * Stopwatch.Frequency) / 1_000_000;
        //static long TicksToMicroseconds(int ticks) => (ticks * 1_000_000) / Stopwatch.Frequency;

        // Initial time estimates may be grossly over estimate the time per row,
        // given JIT, tiering JITing etc.

        var initialBefore = Stopwatch.GetTimestamp();
        var initialResult = select(reader.Current);
        var initialAfter = Stopwatch.GetTimestamp();
        var initialDiff = initialAfter - initialBefore;

        yield return initialResult;

        const int QuantaMicroseconds = 2000;
        // Enumerate rows for quanta to estimate time per row, if more than
        // quanta measured for one, no more will be enumerated
        var quantaTicks = MicrosecondsToTicks(QuantaMicroseconds);
        var rowsPerWorker = (int)Math.Min(int.MaxValue, Math.Max(1, Math.Min(maxRowsPerWorker, quantaTicks / initialDiff)));
        if (rowsPerWorker > 1)
        {
            var results = new List<T>(rowsPerWorker);
            var estimateBefore = Stopwatch.GetTimestamp();
            for (var i = 0; i < rowsPerWorker && (readerHasMore = reader.MoveNext()); i++)
            {
                results.Add(select(reader.Current));
            }
            if (results.Count > 0)
            {
                var estimateAfter = Stopwatch.GetTimestamp();
                foreach (var result in results)
                {
                    yield return result;
                }
                var estimateDiff = estimateAfter - estimateBefore;
                var estimateDiffPerRow = estimateDiff / results.Count;
                rowsPerWorker = (int)Math.Min(int.MaxValue, Math.Max(1, Math.Min(maxRowsPerWorker, quantaTicks / estimateDiffPerRow)));
            }
        }
        if (!readerHasMore) { yield break; }

        using var someWorkItemsDoneEvent = new AutoResetEvent(false);
        Action<int> enqueueDone = workerIndex => { someWorkItemsDoneEvent.Set(); };

        var workersReady = new Stack<int>(maxDegreeOfParallelism);
        var workersExecutingOrdered = new Queue<int>(maxDegreeOfParallelism);

        var workers = new List<RowStatesThreadPoolWorkItem<T>>(maxDegreeOfParallelism);
        for (var workerIndex = 0; workerIndex < maxDegreeOfParallelism; workerIndex++)
        {
            var protoRowState = new SepReaderRowState(reader);
            var worker = new RowStatesThreadPoolWorkItem<T>(protoRowState, rowsPerWorker, select, workerIndex, enqueueDone);
            workers.Add(worker);
            workersReady.Push(workerIndex);
        }

        try
        {
            while (readerHasMore || workersExecutingOrdered.Count > 0)
            {
                while (workersExecutingOrdered.TryPeek(out var workerIndexHead) && workers[workerIndexHead].IsDone)
                {
                    workersExecutingOrdered.TryDequeue(out var expected);
                    Debug.Assert(workerIndexHead == expected);
                    var worker = workers[workerIndexHead];
                    foreach (var result in worker.Results)
                    {
                        yield return result;
                    }
                    worker.IsDone = false;
                    workersReady.Push(workerIndexHead);
                }

                if (readerHasMore && workersReady.TryPop(out var workerIndexNext))
                {
                    var worker = workers[workerIndexNext];
                    Debug.Assert(worker.IsDone == false);
                    var rowStates = worker.RowStates;
                    var rowIndex = 0;
                    for (; rowIndex < rowStates.Count && (readerHasMore = reader.MoveNext()); rowIndex++)
                    {
                        var rowState = rowStates[rowIndex];
                        reader.CopyNewRowTo(rowState);
                    }
                    if (rowIndex > 0)
                    {
                        worker.RowsToExecute = rowIndex;
                        workersExecutingOrdered.Enqueue(workerIndexNext);
                        ThreadPool.UnsafeQueueUserWorkItem(worker, preferLocal: false);
                    }
                    else
                    {
                        workersReady.Push(workerIndexNext);
                    }
                }
                else
                {
                    someWorkItemsDoneEvent.WaitOne(10);
                }
            }
        }
        finally
        {
            foreach (var worker in workers)
            {
                worker.Dispose();
            }
        }
        Debug.Assert(workers.Count == workersReady.Count);
        Debug.Assert(workersExecutingOrdered.Count == 0);
    }

    static IEnumerable<T> ParallelEnumerateInternalManyRowStates<T>(this SepReader reader, SepReader.RowFunc<T> select,
        int maxDegreeOfParallelism, int maxRowsInPlay = 16 * 1024)
    {
        var rowStates = new List<SepReaderRowStateForParallel<T>>(maxDegreeOfParallelism);

        var workers = new List<LoopThreadPoolWorkItem<T>>(maxDegreeOfParallelism);
        var readyWorkers = new ConcurrentBag<LoopThreadPoolWorkItem<T>>();

        var rowStatesUnused = new Stack<int>(maxDegreeOfParallelism);
        var rowStatesExecutingOrdered = new Queue<int>(maxDegreeOfParallelism);
        var rowStatesForExecuting = new ConcurrentQueue<SepReaderRowStateForParallel<T>>();

        using var someWorkItemsDoneEvent = new AutoResetEvent(false);
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

        using var someWorkItemsDoneEvent = new AutoResetEvent(false);
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
            var count = 0;
            while (_rowStatesForExecuting.TryDequeue(out var rowState))
            {
                rowState.Execute();
                _signal(rowState.Index);
                ++count;
            }
            //Log?.Invoke($"TEST Executed {count}");
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
