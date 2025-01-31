using System.Diagnostics.CodeAnalysis;

namespace nietras.SeparatedValues;

[ExcludeFromCodeCoverage]
readonly record struct SepRange(int Start, int Length);
