#define SYNC
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if !SYNC
using System.Threading.Tasks;
#endif

namespace nietras.SeparatedValues;

public sealed partial class SepReader
{
#if SYNC
    internal void Initialize(in SepReaderOptions options)
#else
    internal async ValueTask InitializeAsync(SepReaderOptions options)
#endif
    {
        // Parse first row/header
#if SYNC
        if (MoveNext())
#else
        if (await MoveNextAsync())
#endif
        {
            A.Assert(_parsedRowsCount > 0);

            IsEmpty = false;
            var firstRowColCount = _parsedRows[0].ColCount;
            _colCountExpected = firstRowColCount;
            if (options.HasHeader)
            {
                var headerRow = new string(RowSpan());
                var colNameComparer = options.ColNameComparer;
                var colNameToIndex = new Dictionary<string, int>(firstRowColCount, colNameComparer);
                for (var colIndex = 0; colIndex < firstRowColCount; colIndex++)
                {
                    var colName = ToStringDirect(colIndex);
                    if (!colNameToIndex.TryAdd(colName, colIndex))
                    {
                        SepThrow.ArgumentException_DuplicateColNamesFound(this, colNameToIndex,
                            colName, firstRowColCount, colNameComparer, headerRow);
                    }
                }
                _header = new(headerRow, colNameToIndex);

                HasHeader = true;

                // Check if more data available and hence minimum 1 row after header
                // What if \n after \r after header only? Where \n lingering after MoveNext?
                HasRows = _parsedRowsCount > 1 || _charsDataEnd > _charsParseStart || _parsingRowColCount > 0;
                if (!HasRows)
                {
#if SYNC
                    HasRows = !FillAndMaybeDoubleCharsBuffer(_charsPaddingLength);
#else
                    HasRows = !await FillAndMaybeDoubleCharsBufferAsync(_charsPaddingLength);
#endif
                }
            }
            else
            {
                // Move back one as no header (since MoveNext called twice then)
                --_rowIndex;
                --_parsedRowIndex;
                _currentRowColEndsOrInfosOffset = 0;
                _currentRowColCount = -1;
                A.Assert(_rowIndex == -1);
                A.Assert(_parsedRowIndex == 0);
                HasHeader = false;
                HasRows = true;
            }
            A.Assert(_separator != 0);
        }
        else
        {
            // Nothing in file
            IsEmpty = true;
            HasHeader = false;
            HasRows = false;
            _colCountExpected = 0;
            _separator = Sep.Default.Separator;
        }

        _colNameCache = new (string colName, int colIndex)[_colCountExpected];

        // Header may be null here
        _toString = options.CreateToString(_header, _colCountExpected);

        // Use empty header if no header
        _header ??= SepReaderHeader.Empty;

        _colCountExpected = options.DisableColCountCheck ? -1 : _colCountExpected;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if SYNC
    public bool MoveNext()
#else
    public async ValueTask<bool> MoveNextAsync()
#endif
    {
        do
        {
            if (MoveNextAlreadyParsed())
            {
                return true;
            }
#if SYNC
        } while (ParseNewRows());
#else
        } while (await ParseNewRowsAsync());
#endif
        return false;
    }
}
