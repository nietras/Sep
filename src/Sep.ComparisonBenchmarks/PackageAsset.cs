using System;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

public sealed class PackageAsset
{
    public required Guid? ScanId { get; init; }
    public required DateTimeOffset? ScanTimestamp { get; init; }
    public required string Id { get; init; }
    public required string Version { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string ResultType { get; init; }

    public required string PatternSet { get; init; }
    public required string PropertyAnyValue { get; init; }
    public required string PropertyCodeLanguage { get; init; }
    public required string PropertyTargetFrameworkMoniker { get; init; }
    public required string PropertyLocale { get; init; }
    public required string PropertyManagedAssembly { get; init; }
    public required string PropertyMSBuild { get; init; }
    public required string PropertyRuntimeIdentifier { get; init; }
    public required string PropertySatelliteAssembly { get; init; }

    public required string Path { get; init; }
    public required string FileName { get; init; }
    public required string FileExtension { get; init; }
    public required string TopLevelFolder { get; init; }

    public required string RoundTripTargetFrameworkMoniker { get; init; }
    public required string FrameworkName { get; init; }
    public required string FrameworkVersion { get; init; }
    public required string FrameworkProfile { get; init; }
    public required string PlatformName { get; init; }
    public required string PlatformVersion { get; init; }

    public static PackageAsset Read<TRow>(in TRow row, Func<TRow, int, string> getField)
    {
        Contract.Assume(getField is not null);
        const int offset = 0;
        return new()
        {
            ScanId = ParseNullableGuid(getField(row, 0 + offset)),
            ScanTimestamp = ParseNullableDateTimeOffset(getField(row, 1 + offset)),
            Id = getField(row, 2 + offset),
            Version = getField(row, 3 + offset),
            Created = ParseDateTimeOffset(getField(row, 4 + offset)),
            ResultType = getField(row, 5 + offset),
            PatternSet = getField(row, 6 + offset),
            PropertyAnyValue = getField(row, 7 + offset),
            PropertyCodeLanguage = getField(row, 8 + offset),
            PropertyTargetFrameworkMoniker = getField(row, 9 + offset),
            PropertyLocale = getField(row, 10 + offset),
            PropertyManagedAssembly = getField(row, 11 + offset),
            PropertyMSBuild = getField(row, 12 + offset),
            PropertyRuntimeIdentifier = getField(row, 13 + offset),
            PropertySatelliteAssembly = getField(row, 14 + offset),
            Path = getField(row, 15 + offset),
            FileName = getField(row, 16 + offset),
            FileExtension = getField(row, 17 + offset),
            TopLevelFolder = getField(row, 18 + offset),
            RoundTripTargetFrameworkMoniker = getField(row, 19 + offset),
            FrameworkName = getField(row, 20 + offset),
            FrameworkVersion = getField(row, 21 + offset),
            FrameworkProfile = getField(row, 22 + offset),
            PlatformName = getField(row, 23 + offset),
            PlatformVersion = getField(row, 24 + offset),
        };
    }

    static DateTimeOffset? ParseNullableDateTimeOffset(ReadOnlySpan<char> input) =>
        input.Length > 0 ? ParseDateTimeOffset(input) : null;

    static DateTimeOffset ParseDateTimeOffset(ReadOnlySpan<char> input) =>
        DateTimeOffset.ParseExact(input, "O", CultureInfo.InvariantCulture);

    static Guid? ParseNullableGuid(ReadOnlySpan<char> input) =>
        input.Length > 0 ? Guid.Parse(input) : null;
}
