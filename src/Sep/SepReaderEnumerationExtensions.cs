//#define SEPTRACEPARALLEL
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    // TODO: Add SepReaderParallelOptions
    public static IEnumerable<T> ParallelEnumerate<T>(this SepReader reader, SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
    {
        if (maxDegreeOfParallelism <= 1) { return Enumerate(reader, select); }
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(select);
        if (!reader.HasRows) { return Array.Empty<T>(); }
        return ParallelEnumerateAsParallelManyRows(reader, select, maxDegreeOfParallelism);
    }

    static IEnumerable<T> ParallelEnumerateAsParallelManyRows<T>(this SepReader reader,
        SepReader.RowFunc<T> select, int maxDegreeOfParallelism)
    {
        var statesStack = new ConcurrentStack<SepReaderState>();
        try
        {
            var states = EnumerateStates(reader, statesStack);
            var results = states.AsParallel()
                                .AsOrdered()
                                .WithDegreeOfParallelism(maxDegreeOfParallelism)
                                .SelectMany(EnumerateParsedRows);
            foreach (var result in results)
            {
                yield return result;
            }
        }
        finally
        {
            //Log($"States count {states.Count}");
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

        IEnumerable<T> EnumerateParsedRows(SepReaderState s)
        {
            while (s.MoveNextAlreadyParsed())
            {
                var result = select(new(s));
                yield return result;
            }
            //Log($"T:{Environment.CurrentManagedThreadId,2} ParsedRows: {s._parsedRowsCount,5} ColInfos {s._currentRowColEndsOrInfosStartIndex,5} S: {s._charsDataStart,6} P: {s._charsParseStart,6} E: {s._charsDataEnd,6}");
            statesStack.Push(s);
        }
    }
}
