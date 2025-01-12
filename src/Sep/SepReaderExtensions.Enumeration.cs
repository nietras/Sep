//#define SEPTRACEPARALLEL
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#if NET9_0_OR_GREATER
using System.Threading.Tasks;
#endif

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

#if NET9_0_OR_GREATER
    public static async IAsyncEnumerable<T> EnumerateAsync<T>(this SepReader reader, Func<SepReader.Row, ValueTask<T>> select)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);
        await foreach (var row in reader)
        {
            yield return await select(row).ConfigureAwait(false);
        }
    }

    public static async IAsyncEnumerable<T> EnumerateAsync<T>(this SepReader reader, Func<SepReader.Row, ValueTask<(bool, T)>> trySelect)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(trySelect);
        await foreach (var row in reader)
        {
            var (success, value) = await trySelect(row).ConfigureAwait(false);
            if (success)
            {
                yield return value;
            }
        }
    }
#endif

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

    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.RowFunc<T> select, int degreeOfParallism)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);
        if (!reader.HasRows) { return Array.Empty<T>(); }
        return ParallelEnumerateAsParallel(reader, select, p => p.WithDegreeOfParallelism(degreeOfParallism));
    }

    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.RowTryFunc<T> trySelect, int degreeOfParallism)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(trySelect);
        if (!reader.HasRows) { return Array.Empty<T>(); }
        return ParallelEnumerateAsParallel(reader, trySelect, p => p.WithDegreeOfParallelism(degreeOfParallism));
    }

    static IEnumerable<T> ParallelEnumerateAsParallel<T>(this SepReader reader, SepReader.RowFunc<T> select,
        Func<ParallelQuery<SepReaderState>, ParallelQuery<SepReaderState>>? modifyParallelQuery = null)
    {
        var statesStack = new ConcurrentStack<SepReaderState>();
        try
        {
            var parallelStates = EnumerateStatesParallel(reader, statesStack, modifyParallelQuery);
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

    static IEnumerable<T> ParallelEnumerateAsParallel<T>(this SepReader reader, SepReader.RowTryFunc<T> trySelect,
        Func<ParallelQuery<SepReaderState>, ParallelQuery<SepReaderState>>? modifyParallelQuery = null)
    {
        var statesStack = new ConcurrentStack<SepReaderState>();
        try
        {
            var parallelStates = EnumerateStatesParallel(reader, statesStack, modifyParallelQuery);
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

    static ParallelQuery<SepReaderState> EnumerateStatesParallel(SepReader reader,
        ConcurrentStack<SepReaderState> statesStack,
        Func<ParallelQuery<SepReaderState>, ParallelQuery<SepReaderState>>? modifyParallelQuery = null)
    {
        var states = EnumerateStates(reader, statesStack);
        // For now always force ordered
        var statesParallel = states.AsParallel().AsOrdered();
        return modifyParallelQuery is null ? statesParallel : modifyParallelQuery(statesParallel);
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
                reader.SwapParsedRowsTo(state);
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
