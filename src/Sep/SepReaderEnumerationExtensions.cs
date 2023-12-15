//#define SEPTRACEPARALLEL
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace nietras.SeparatedValues;

public static class SepReaderEnumerationExtensions
{
#if SEPTRACEPARALLEL
    static readonly Action<string> Log = t => { Console.WriteLine(t); T.WriteLine(t); };
#endif
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

    // TODO: Add SepReaderParallelOptions
    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.RowFunc<T> select)
    {
        //if (maxDegreeOfParallelism <= 1) { return Enumerate(reader, select); }
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);
        if (!reader.HasRows) { return Array.Empty<T>(); }
        return ParallelEnumerateAsParallel(reader, select);
    }

    static IEnumerable<T> ParallelEnumerateAsParallel<T>(this SepReader reader, SepReader.RowFunc<T> select)
    {
        var statesStack = new ConcurrentStack<SepReaderState>();
        try
        {
            var states = EnumerateStates(reader, statesStack);
            var parallel = states.AsParallel()
                                 .AsOrdered()
                                 //.WithDegreeOfParallelism(maxDegreeOfParallelism)
                                 ;
            // Pooled has better perf and much fewer allocations
            //const bool pooled = true;
            //if (pooled)
            //{
            var batches = parallel.Select(GetParsedRowsPooled);
            foreach (var batch in batches)
            {
                var (results, count) = batch;
                for (var i = 0; i < count; i++)
                {
                    yield return results[i];
                }
                ArrayPool<T>.Shared.Return(results);
            }
            //}
            //else
            //{
            //    var results = parallel.SelectMany(EnumerateParsedRows);
            //    foreach (var result in results)
            //    {
            //        yield return result;
            //    }
            //}
        }
        finally
        {
#if SEPTRACEPARALLEL
            Log($"States stack count {statesStack.Count}");
#endif
            foreach (var state in statesStack)
            {
                state.Dispose();
            }
        }

        IEnumerable<SepReaderState> EnumerateStates(SepReader reader, ConcurrentStack<SepReaderState> states)
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

        (T[] Array, int Count) GetParsedRowsPooled(SepReaderState s)
        {
            var array = ArrayPool<T>.Shared.Rent(s._parsedRowsCount);
            var index = 0;
            while (s.MoveNextAlreadyParsed())
            {
                array[index] = select(new(s));
                ++index;
            }
            statesStack.Push(s);
            return (array, index);
        }

        //        IEnumerable<T> EnumerateParsedRows(SepReaderState s)
        //        {
        //            while (s.MoveNextAlreadyParsed())
        //            {
        //                var result = select(new(s));
        //                yield return result;
        //            }
        //#if SEPTRACEPARALLEL
        //            Log($"T:{Environment.CurrentManagedThreadId,2} ParsedRows: {s._parsedRowsCount,5} ColInfos {s._currentRowColEndsOrInfosStartIndex,5} S: {s._charsDataStart,6} P: {s._charsParseStart,6} E: {s._charsDataEnd,6}");
        //#endif
        //            statesStack.Push(s);
        //        }
    }
}
