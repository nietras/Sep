// Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
#pragma warning disable CA1852
using System;
using System.Diagnostics;
using nietras.SeparatedValues;
using static System.Console;
[assembly: System.Runtime.InteropServices.ComVisible(false)]

OutputEncoding = System.Text.Encoding.UTF8;
Action<string> log = t => { WriteLine(t); Trace.WriteLine(t); };

var text = """
            A;B;C;D;E;F
            Sep;🚀;1;1.2;0.1;0.5
            CSV;✅;2;2.2;0.2;1.5
            """;

using var reader = Sep.Reader().FromText(text);   // Infers separator 'Sep' from header
using var writer = reader.Spec.Writer().ToText(); // Writer defined from reader 'Sep'
                                                  // Use .FromFile(...)/ToFile(...) for files
var idx = reader.Header.IndexOf("B");
var nms = new[] { "E", "F" };

foreach (var readRow in reader)           // Read one row at a time
{
    var a = readRow["A"].Span;            // Column as ReadOnlySpan<char>
    var b = readRow[idx].ToString();      // Column to string (allocates new string per call)
    var c = readRow["C"].Parse<int>();    // Parse any T : ISpanParsable<T>
    var d = readRow["D"].Parse<float>();  // Parse float/double fast via csFastFloat
    var s = readRow[nms].Parse<double>(); // Parse multiple columns as Span<T>
                                          // - Sep handles array allocation and reuse
    foreach (ref var v in s) { v *= 10; }

    using var writeRow = writer.NewRow(); // Start new row. Row written on Dispose.
    writeRow["A"].Set(a);                 // Set by ReadOnlySpan<char>
    writeRow["B"].Set(b);                 // Set by string
    writeRow["C"].Set($"{c * 2}");        // Set via InterpolatedStringHandler, no allocs
    writeRow["D"].Format(d / 2);          // Format any T : ISpanFormattable
    writeRow[nms].Format(s);              // Format multiple columns directly
                                          // Columns are added on first access as ordered
}

log("=== Input  ===");
log(text);
log("=== Output ===");
log(writer.ToString());

// Above example code is for demonstration purposes only.
// Short names and repeated constants are only for demonstration.
