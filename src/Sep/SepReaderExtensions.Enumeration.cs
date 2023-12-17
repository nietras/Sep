﻿//#define SEPTRACEPARALLEL
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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

    public static IEnumerable<T> Enumerate<T>(this SepReader reader, SepReader.TryRowFunc<T> trySelect)
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
        //return ParallelEnumerateAsParallelParseTask(reader, select);
    }

    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.TryRowFunc<T> trySelect)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(trySelect);
        if (!reader.HasRows) { return Array.Empty<T>(); }
        return ParallelEnumerateAsParallel(reader, trySelect);
    }

    static IEnumerable<T> ParallelEnumerateAsParallelParseTask<T>(this SepReader reader, SepReader.RowFunc<T> select)
    {
        var statesStack = new ConcurrentStack<SepReaderState>();
        var statesForProcesssing = new BlockingCollection<SepReaderState>();
        try
        {
            var parseTask = Task.Run(() =>
            {
                foreach (var state in EnumerateStates(reader, statesStack))
                {
                    statesForProcesssing.Add(state);
                }
                statesForProcesssing.CompleteAdding();
            });

            var batches = statesForProcesssing.GetConsumingEnumerable()
                                              .AsParallel()
                                              .AsOrdered()
                                              .Select(PooledSelect);
            foreach (var batch in batches)
            {
                var (results, count) = batch;
                for (var i = 0; i < count; i++)
                {
                    yield return results[i];
                }
                ArrayPool<T>.Shared.Return(results);
            }

            Debug.Assert(parseTask.IsCompleted);
            parseTask.GetAwaiter().GetResult();
        }
        finally
        {
            DisposeStates(statesStack);
            statesForProcesssing.Dispose();
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

    static IEnumerable<T> ParallelEnumerateAsParallel<T>(this SepReader reader, SepReader.TryRowFunc<T> trySelect)
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
            if (!states.TryPop(out var state))
            {
                state = new SepReaderState(reader);
            }
            if (reader.HasParsedRows())
            {
                reader.CopyParsedRowsTo(state);
                yield return state;
            }
        } while (reader.ParseNewRows());
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
