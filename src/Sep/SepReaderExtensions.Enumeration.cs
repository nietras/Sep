//#define SEPTRACEPARALLEL
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace nietras.SeparatedValues;

public static partial class SepReaderExtensions
{
#if SEPTRACEPARALLEL
    static readonly Action<string> Log = t => { Console.WriteLine(t); T.WriteLine(t); };
#endif

    public static IEnumerable<T> Enumerate<T>(this SepReader reader, SepReader.RowFunc<T> select)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);
        foreach (var row in reader)
        {
            yield return select(row);
        }
    }

    public static IEnumerable<T> Enumerate<T>(this SepReader reader, SepReader.RowTryFunc<T> trySelect)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(trySelect);
        foreach (var row in reader)
        {
            if (trySelect(row, out var value))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.RowFunc<T> select)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);
        if (!reader.HasRows) { return Array.Empty<T>(); }
        return ParallelEnumerateAsParallel(reader, select);
    }

    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.RowTryFunc<T> trySelect)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(trySelect);
        if (!reader.HasRows) { return Array.Empty<T>(); }
        return ParallelEnumerateAsParallel(reader, trySelect);
    }

    static IEnumerable<T> ParallelEnumerateAsParallel<T>(this SepReader reader, SepReader.RowFunc<T> select)
    {
        var statesStack = new ConcurrentStack<SepReaderState>();
        try
        {
            var parallelStates = EnumerateStatesParallel(reader, statesStack);
            var batches = parallelStates.Select(PooledSelect);
            foreach (var batch in batches)
            {
                var (results, count) = batch;
                for (var i = 0; i < count; i++)
                {
                    yield return results[i];
                }
                ArrayPool<T>.Shared.Return(results);
            }
        }
        finally
        {
            DisposeStates(statesStack);
        }

        (T[] Array, int Count) PooledSelect(SepReaderState s)
        {
            var array = ArrayPool<T>.Shared.Rent(s._parsedRowsCount);
            var index = 0;
            while (s.MoveNextAlreadyParsed())
            {
                array[index] = select(new(s));
                ++index;
            }
#if SEPTRACEPARALLEL
            Log($"T:{Environment.CurrentManagedThreadId,2} ParsedRows: {s._parsedRowsCount,5} ColInfos {s._currentRowColEndsOrInfosStartIndex,5} S: {s._charsDataStart,6} P: {s._charsParseStart,6} E: {s._charsDataEnd,6}");
#endif
            statesStack.Push(s);
            return (array, index);
        }
    }

    static IEnumerable<T> ParallelEnumerateAsParallel<T>(this SepReader reader, SepReader.RowTryFunc<T> trySelect)
    {
        var statesStack = new ConcurrentStack<SepReaderState>();
        try
        {
            var parallelStates = EnumerateStatesParallel(reader, statesStack);
            var batches = parallelStates.Select(PooledSelect);
            foreach (var batch in batches)
            {
                var (results, count) = batch;
                for (var i = 0; i < count; i++)
                {
                    yield return results[i];
                }
                ArrayPool<T>.Shared.Return(results);
            }
        }
        finally
        {
            DisposeStates(statesStack);
        }

        (T[] Array, int Count) PooledSelect(SepReaderState s)
        {
            var array = ArrayPool<T>.Shared.Rent(s._parsedRowsCount);
            var index = 0;
            while (s.MoveNextAlreadyParsed())
            {
                if (trySelect(new(s), out var value))
                {
                    array[index] = value;
                    ++index;
                }
            }
#if SEPTRACEPARALLEL
            Log($"T:{Environment.CurrentManagedThreadId,2} ParsedRows: {s._parsedRowsCount,5} ColInfos {s._currentRowColEndsOrInfosStartIndex,5} S: {s._charsDataStart,6} P: {s._charsParseStart,6} E: {s._charsDataEnd,6}");
#endif
            statesStack.Push(s);
            return (array, index);
        }
    }
    static ParallelQuery<SepReaderState> EnumerateStatesParallel(SepReader reader, ConcurrentStack<SepReaderState> statesStack)
    {
        var states = EnumerateStates(reader, statesStack);
        return states.AsParallel().AsOrdered();
    }

    static IEnumerable<SepReaderState> EnumerateStates(SepReader reader, ConcurrentStack<SepReaderState> states)
    {
        do
        {
            if (reader.HasParsedRows())
            {
                if (!states.TryPop(out var state))
                {
                    state = new SepReaderState(reader);
                }
                reader.SwapParsedRowsTo(state);
                yield return state;
            }
        } while (reader.ParseNewRows());
    }

    static IEnumerable<SepReaderState> EnumerateStatesByChunksBacktrackingNewLine(SepReader reader, ConcurrentStack<SepReaderState> states)
    {
        // Return initial sequentially parsed state
        if (reader.HasParsedRows())
        {
            if (!states.TryPop(out var state))
            {
                state = new SepReaderState(reader);
            }
            reader.SwapParsedRowsTo(state);
            yield return state;
        }

        // Move lingering data to start in reader
        // Fill buffer
        // Check if any data or finish (could then avoid parallelization for that case - short file)
        // Find last new line (\r, \r\n, \n) by backtracking (must always have 1 char after new line)
        // Take care of end-of-file where last row may not have new line
        // Set data end to just before?/after? new line (be sure next start then is after)
        // Swap to state and let parsing be done in parallel
        // Continue producing states until no more data
        while (reader.PrepareAndReadDataForNewRows())
        {
            if (!states.TryPop(out var state))
            {
                state = new SepReaderState(reader);
            }
            reader.SwapDataForParsingNewRows(state);
            yield return state;
            // TODO: Call parse rows after with check for
        }
    }

    static void DisposeStates(ConcurrentStack<SepReaderState> statesStack)
    {
#if SEPTRACEPARALLEL
            Log($"States stack count {statesStack.Count}");
#endif
        foreach (var state in statesStack)
        {
            state.Dispose();
        }
    }
}
