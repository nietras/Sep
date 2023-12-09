//#define SEPTRACEPARALLEL
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace nietras.SeparatedValues;

public static class SepReaderEnumerationExtensions
{
    static readonly Action<string> Log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

    // TODO: Finalize and expose public with TryRowFunc too
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
        return ParallelEnumerateAsParallelManyRows(reader, select, maxDegreeOfParallelism);
        //return ParallelEnumerateInternalBusyLooping(reader, select, maxDegreeOfParallelism);
        // Fastest at the moment but still slower than single threaded
        //return ParallelEnumerateInternalRowStatesGroupPerWorker(reader, select, maxDegreeOfParallelism);
        // Extremely slow
        //return ParallelEnumerateInternalManyRowStates(reader, select, maxDegreeOfParallelism);
        // Appears not to work
        //return ParallelEnumerateInternalOneWorkItemPerRow(reader, select, maxDegreeOfParallelism);
    }

    static IEnumerable<T> ParallelEnumerateAsParallelManyRows<T>(this SepReader reader,
        SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
    {
        //maxDegreeOfParallelism *= 16;
        var states = new ConcurrentStack<SepReaderState>();
        try
        {
            foreach (var result in EnumerateStates(reader, states)
                .AsParallel()
                .AsOrdered()
                .WithDegreeOfParallelism(maxDegreeOfParallelism)
                //.WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .SelectMany(EnumerateParsedRows))
            {
                yield return result;
            }
        }
        finally
        {
            //Console.WriteLine($"States count {states.Count}");
            foreach (var state in states)
            {
                state.Dispose();
            }
        }

        IEnumerable<SepReaderState> EnumerateStates(SepReader reader, ConcurrentStack<SepReaderState> states)
        {
            var createdCount = 0;
            //using (states)
            {
                //for (var i = 0; i < maxDegreeOfParallelism / 4; i++)
                //{
                //    var state = new SepReaderState(reader);
                //    states.Push(state);
                //    ++createdCount;
                //}
                do
                {
                    if (!states.TryPop(out var state))
                    {
                        state = new SepReaderState(reader);
                        ++createdCount;
                    }
                    if (reader.HasParsedRows())
                    {
                        reader.CopyParsedRowsTo(state);
                        yield return state;
                    }
                } while (reader.ParseNewRows());

                for (var i = 0; i < createdCount; i++)
                {
                    //if (states.TryTake(out var state))
                    //var state = states.Take();
                    //{
                    //    state.Dispose();
                    //}
                    //Trace.WriteLine("Dispose");
                }
            }
        }

        IEnumerable<T> EnumerateParsedRows(SepReaderState s)
        {
            while (s.MoveNextAlreadyParsed())
            {
                var result = select(new(s));
                yield return result;
            }
            //Console.WriteLine($"T:{Environment.CurrentManagedThreadId,2} ParsedRows: {s._parsedRowsCount,5} ColInfos {s._currentRowColEndsOrInfosStartIndex,5} S: {s._charsDataStart,6} P: {s._charsParseStart,6} E: {s._charsDataEnd,6}");
            states.Push(s);
        }
    }

    static IEnumerable<T> ParallelEnumerateAsParallel<T>(this SepReader reader,
        SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
    {
        //maxDegreeOfParallelism *= 16;
        var states = new BlockingCollection<SepReaderState>();
        return EnumerateStates(reader, states)
            .AsParallel()
            //.WithDegreeOfParallelism(maxDegreeOfParallelism)
            //.WithMergeOptions(ParallelMergeOptions.NotBuffered)
            .AsOrdered()
            .Select(s =>
            {
                var result = select(new(s));
                states.Add(s);
                //Trace.WriteLine("Add");
                return result;
            });

        IEnumerable<SepReaderState> EnumerateStates(SepReader reader, BlockingCollection<SepReaderState> states)
        {
            var createdCount = 0;
            //using (states)
            {
                for (var i = 0; i < maxDegreeOfParallelism; i++)
                {
                    var state = new SepReaderState(reader);
                    states.Add(state);
                    ++createdCount;
                }

                foreach (var row in reader)
                {
                    if (!states.TryTake(out var state))
                    {
                        state = new SepReaderState(reader);
                        ++createdCount;
                    }

                    reader.CopyNewRowTo(state);
                    //Trace.WriteLine("Yield");
                    yield return state;
                }

                for (var i = 0; i < createdCount; i++)
                {
                    //if (states.TryTake(out var state))
                    //var state = states.Take();
                    //{
                    //    state.Dispose();
                    //}
                    //Trace.WriteLine("Dispose");
                }
            }
        }
    }

    class KeepDequeueThreadPoolWorkItem<T> : IThreadPoolWorkItem
    {
        readonly ConcurrentQueue<SepReaderRowStateForParallel<T>> _statesForExecuting;
        //readonly SemaphoreSlim _semaphoreSlim;
        readonly ConcurrentBag<KeepDequeueThreadPoolWorkItem<T>> _doneWorkItems;
        readonly Action<int> _signal;

        public KeepDequeueThreadPoolWorkItem(
            ConcurrentQueue<SepReaderRowStateForParallel<T>> statesForExecuting,
            //SemaphoreSlim semaphoreSlim,
            ConcurrentBag<KeepDequeueThreadPoolWorkItem<T>> doneWorkItems,
            Action<int> signal)
        {
            _statesForExecuting = statesForExecuting;
            //_semaphoreSlim = semaphoreSlim;
            _doneWorkItems = doneWorkItems;
            _signal = signal;
        }

        public void Execute()
        {
            var count = 0;
            //while (_semaphoreSlim.Wait(10))
            //{
            //    if (_statesForExecuting.TryDequeue(out var state))
            //    {
            //        state.Execute();
            //        _signal(state.Index);
            //        ++count;
            //    }
            //    else
            //    {
            //        Debug.Assert(false, "semaphore / queue inconsistency");
            //    }
            //}
            while (_statesForExecuting.TryDequeue(out var state))
            {
                state.Execute();
                _signal(state.Index);
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
        var maxRowsCount = workerCount;

        var workers = new List<KeepDequeueThreadPoolWorkItem<T>>(workerCount);
        //var workIndicesReady = new Stack<int>(workerCount);
        //var workIndicesExecutingOrdered = new Queue<int>(workerCount);
        var readyWorkers = new ConcurrentBag<KeepDequeueThreadPoolWorkItem<T>>();

        var states = new SepReaderRowStateForParallel<T>[maxRowsCount];
        var statesUnused = new Stack<int>(maxDegreeOfParallelism);
        var statesExecutingOrdered = new Queue<int>(maxDegreeOfParallelism);
        var statesForExecuting = new ConcurrentQueue<SepReaderRowStateForParallel<T>>();
        //using var statesForExecutingSemaphore = new SemaphoreSlim(initialCount: 0);
        //var statesIndicesForExecuting = new ConcurrentQueue<int>();

        using var someWorkItemsDoneEvent = new AutoResetEvent(false);
        Action<int> enqueueDone = workIndex => { someWorkItemsDoneEvent.Set(); };

        try
        {
            for (var stateIndex = 0; stateIndex < maxRowsCount; stateIndex++)
            {
                var state = new SepReaderRowStateForParallel<T>(reader, stateIndex, select);
                states[stateIndex] = state;
                statesUnused.Push(stateIndex);
            }

            var waitCount = 0;
            var queueCount = 0;

            var readerNext = true;
            while (readerNext || statesExecutingOrdered.Count > 0)
            {
                while (statesForExecuting.Count > workerCount &&
                       workers.Count > 0 && readyWorkers.Count >= workers.Count)
                //while (statesForExecuting.Count >= states.Length &&
                //   workers.Count >= maxDegreeOfParallelism && readyWorkers.Count < 1)
                {
                    //Log?.Invoke($"TEST Waiting {statesExecutingOrdered.Count} {readyWorkers.Count}:{workers.Count}");
                    someWorkItemsDoneEvent.WaitOne(1);
                    ++waitCount;
                }
                // Fill up row states
                while (statesUnused.Count > 0 && readerNext && (readerNext = reader.MoveNext()))
                {
                    var stateIndex = statesUnused.Pop();
                    var state = states[stateIndex];
                    state.IsDone = false;
                    reader.CopyNewRowTo(state);
                    statesExecutingOrdered.Enqueue(stateIndex);
                    statesForExecuting.Enqueue(state);
                    //statesForExecutingSemaphore.Release();
                }
                // Yield results
                SepReaderRowStateForParallel<T>? stateMaybeDone;
                while (statesExecutingOrdered.TryPeek(out var stateIndexNext) &&
                       (stateMaybeDone = states[stateIndexNext]).IsDone)
                {
                    statesExecutingOrdered.Dequeue();
                    yield return stateMaybeDone.Result;
                    statesUnused.Push(stateIndexNext);
                }
                // Add worker if ??
                if (statesForExecuting.Count > ((states.Length - workerCount) / 2) ||
                    readyWorkers.Count == workers.Count)
                {
                    if (!readyWorkers.TryTake(out var worker))
                    {
                        if (workers.Count < maxDegreeOfParallelism)
                        {
                            worker = new KeepDequeueThreadPoolWorkItem<T>(statesForExecuting, /*statesForExecutingSemaphore,*/ readyWorkers, enqueueDone);
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

            ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
            //Log?.Invoke($"Workers {workers.Count} ThreadPoolQueue {queueCount} Wait {waitCount} Rows {reader._rowIndex} ThreadPool {workerThreads}");
        }
        finally
        {
            foreach (var state in states)
            {
                state.Dispose();
            }
        }
    }

    sealed class RowStatesThreadPoolWorkItem<T> : IThreadPoolWorkItem, IDisposable
    {
        readonly SepReaderState _protoRowState;
        readonly int _stateCountPerExecute;
        readonly SepReader.RowFunc<T> _select;
        readonly int _index;
        readonly Action<int> _signal;
        readonly List<SepReaderState> _states;
        readonly List<T> _results;
        bool _disposedValue;

        public RowStatesThreadPoolWorkItem(
            SepReaderState protoRowState, int initialRowStateCount, SepReader.RowFunc<T> select,
            int index, Action<int> signal)
        {
            _protoRowState = protoRowState;
            _stateCountPerExecute = Math.Max(1, initialRowStateCount);
            _select = select;
            _index = index;
            _signal = signal;
            _states = new(initialRowStateCount) { protoRowState };
            EnsureRowStates(initialRowStateCount);
            _results = new(initialRowStateCount);
        }

        public int RowsToExecute { get; set; }
        public List<SepReaderState> RowStates => _states;
        public List<T> Results => _results;
        public volatile bool IsDone = false;

        public void Execute()
        {
#if SEPTRACEPARALLEL
            var b = Stopwatch.GetTimestamp();
#endif
            _results.Clear();
            Debug.Assert(RowsToExecute <= _states.Count);
            for (var i = 0; i < RowsToExecute; i++)
            {
                var state = _states[i];
                state.ResetSharedCache();
                _results.Add(_select(new(state)));
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
            while (_states.Count < maxRowsToExecute)
            {
                _states.Add(_protoRowState.CloneWithSharedCache());
            }
        }

        void DisposeManaged()
        {
            foreach (var state in _states)
            {
                state.Dispose();
            }
            _states.Clear();
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
            var protoRowState = new SepReaderState(reader);
            var worker = new RowStatesThreadPoolWorkItem<T>(protoRowState, rowsPerWorker, select, workerIndex, enqueueDone);
            workers.Add(worker);
            workersReady.Push(workerIndex);
        }

        try
        {
            //var queueCount = 0;
            //var waitCount = 0;
            while (readerHasMore || workersExecutingOrdered.Count > 0)
            {
                var t0 = Stopwatch.GetTimestamp();
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
                var t1 = Stopwatch.GetTimestamp();
                if (readerHasMore && workersReady.TryPop(out var workerIndexNext))
                {
                    var worker = workers[workerIndexNext];
                    Debug.Assert(worker.IsDone == false);
                    var states = worker.RowStates;
                    var rowIndex = 0;
                    for (; rowIndex < states.Count && (readerHasMore = reader.MoveNext()); rowIndex++)
                    {
                        var state = states[rowIndex];
                        reader.CopyNewRowTo(state);
                    }
                    if (rowIndex > 0)
                    {
                        worker.RowsToExecute = rowIndex;
                        workersExecutingOrdered.Enqueue(workerIndexNext);
                        ThreadPool.UnsafeQueueUserWorkItem(worker, preferLocal: false);
                        //++queueCount;
                    }
                    else
                    {
                        workersReady.Push(workerIndexNext);
                    }
                }
                else
                {
                    someWorkItemsDoneEvent.WaitOne(10);
                    //++waitCount;
                }
                var t2 = Stopwatch.GetTimestamp();
                //var d1 = (t1 - t0) * 1000_000.0 / Stopwatch.Frequency;
                //var d2 = (t2 - t1) * 1000_000.0 / Stopwatch.Frequency;
                //Log?.Invoke($"Loop {d1,7:F3} us {d2,7:F3} us");
            }

            //Log?.Invoke($"Workers {workers.Count} ThreadPoolQueue {queueCount} Wait {waitCount} Rows {reader._rowIndex}");
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
        var states = new List<SepReaderRowStateForParallel<T>>(maxDegreeOfParallelism);

        var workers = new List<LoopThreadPoolWorkItem<T>>(maxDegreeOfParallelism);
        var readyWorkers = new ConcurrentBag<LoopThreadPoolWorkItem<T>>();

        var statesUnused = new Stack<int>(maxDegreeOfParallelism);
        var statesExecutingOrdered = new Queue<int>(maxDegreeOfParallelism);
        var statesForExecuting = new ConcurrentQueue<SepReaderRowStateForParallel<T>>();

        using var someWorkItemsDoneEvent = new AutoResetEvent(false);
        Action<int> enqueueDone = workIndex => { someWorkItemsDoneEvent.Set(); };

        try
        {
            var rowsToReadPerLoop = Math.Min(maxDegreeOfParallelism, maxRowsInPlay);
            var readerNext = true;
            while (readerNext || statesExecutingOrdered.Count > 0)
            {
                //workers.Count < maxDegreeOfParallelism ||
                var rowsRead = 0;
                while ((rowsRead < rowsToReadPerLoop && statesExecutingOrdered.Count < maxRowsInPlay)
                       && (readerNext = reader.MoveNext()))
                {
                    SepReaderRowStateForParallel<T> state;
                    if (statesUnused.TryPop(out var statesIndex))
                    {
                        state = states[statesIndex];
                    }
                    else
                    {
                        statesIndex = states.Count;
                        state = new SepReaderRowStateForParallel<T>(reader, statesIndex, select);
                        states.Add(state);
                    }

                    state.IsDone = false;
                    reader.CopyNewRowTo(state);

                    statesExecutingOrdered.Enqueue(statesIndex);
                    statesForExecuting.Enqueue(state);

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
                        statesExecutingOrdered.Count >= maxRowsInPlay)
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
                else if (statesForExecuting.Count > workers.Count && states.Count > workers.Count
                         && workers.Count < maxDegreeOfParallelism)
                {
#if SEPTRACEPARALLEL
                    Trace.WriteLine($"Add worker {nameof(statesForExecuting)}:{statesForExecuting.Count} {nameof(workers)}:{workers.Count}  {nameof(states)}:{states.Count}");
#endif
                    var worker = new LoopThreadPoolWorkItem<T>(statesForExecuting, readyWorkers, enqueueDone);
                    workers.Add(worker);
                    ThreadPool.UnsafeQueueUserWorkItem(worker, preferLocal: false);
                }

                var returnCount = 0;
                SepReaderRowStateForParallel<T> nextRowStateToReturn;
                while (statesExecutingOrdered.TryPeek(out var nextRowStateIndexToReturn) &&
                       (nextRowStateToReturn = states[nextRowStateIndexToReturn]).IsDone)
                {
                    var index = statesExecutingOrdered.Dequeue();
                    Debug.Assert(nextRowStateIndexToReturn == index);
                    Debug.Assert(nextRowStateIndexToReturn == nextRowStateToReturn.Index);
                    yield return nextRowStateToReturn.Result;
                    statesUnused.Push(nextRowStateIndexToReturn);
                    ++returnCount;
                }

                if (states.Count >= maxRowsInPlay)
                {
                    someWorkItemsDoneEvent.WaitOne(100);
                }
            }
        }
        finally
        {
            foreach (var state in states)
            {
                state.Dispose();
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

        var statesIndicesForExecuting = new ConcurrentQueue<int>();

        try
        {
            for (var workIndex = 0; workIndex < workerCount; workIndex++)
            {
                var state = new SepReaderRowStateForParallel<T>(reader, workIndex, select);
                workItems[workIndex] = new RowThreadPoolWorkItem<T>(state, select, workIndex, enqueueDone);
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

    sealed class SepReaderRowStateForParallel<T> : SepReaderState
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
        readonly ConcurrentQueue<SepReaderRowStateForParallel<T>> _statesForExecuting;
        readonly ConcurrentBag<LoopThreadPoolWorkItem<T>> _doneWorkItems;
        readonly Action<int> _signal;

        public LoopThreadPoolWorkItem(
            ConcurrentQueue<SepReaderRowStateForParallel<T>> statesForExecuting,
            ConcurrentBag<LoopThreadPoolWorkItem<T>> doneWorkItems,
            Action<int> signal)
        {
            _statesForExecuting = statesForExecuting;
            _doneWorkItems = doneWorkItems;
            _signal = signal;
        }

        public void Execute()
        {
            var count = 0;
            while (_statesForExecuting.TryDequeue(out var state))
            {
                state.Execute();
                _signal(state.Index);
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
            SepReaderRowStateForParallel<T> state, SepReader.RowFunc<T> select,
            int workIndex, Action<int> signalDone)
        {
            RowState = state;
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
