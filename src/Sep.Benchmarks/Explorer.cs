using System;
using System.Collections.Generic;

namespace nietras.SeparatedValues.Benchmarks;

public static class Explorer
{
    public static void ExploreUsage(Action<string> log)
    {
        var allNumberToName = new Dictionary<int, string>();

        using var names = Sep.Reader().FromFile(@"ProductId_Names.txt");
        foreach (var row in names)
        {
            var number = row["varenummer"].Parse<int>();
            var name = row["varetekst"].ToString();
            allNumberToName.Add(number, name);
        }

        using var numbersAndNames = Sep.Reader().FromFile(@"production-dump-named.csv");
        var numberToName = new SortedDictionary<int, string>();
        foreach (var row in numbersAndNames)
        {
            if (row["ProductInternalID"].TryParse<int>(out var number) && !numberToName.ContainsKey(number))
            {
                var name = row["ProductName"].ToString();
                name = name.Trim('"').Trim(' ');
                numberToName.Add(number, name);
            }
        }

        using var numbers = Sep.Reader().FromFile(@"production-dump.csv");
        foreach (var row in numbers)
        {
            if (row["productID"].TryParse<int>(out var number) && !numberToName.ContainsKey(number))
            {
                var foundName = allNumberToName.TryGetValue(number, out var n) ? n : "UNKNOWN";
                numberToName.Add(number, foundName);
            }
        }

        using var w = numbers.Spec.Writer().ToFile(@"product-ids-named.csv");
        foreach (var (number, name) in numberToName)
        {
            using var row = w.NewRow();
            row["ProductId"].Format(number);
            row["Name"].Set(name);
        }
    }
}
