#if DEBUG
global using Pos = nietras.SeparatedValues.SepCharPosition;
#else
global using Pos = System.Int32;
#endif
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
#if DEBUG
public
#endif
readonly struct SepCharPosition : IEquatable<SepCharPosition>
{
    internal const int CharShift = 24;
    internal const int MaxLength = 1 << CharShift;
    internal const int MaxPosition = MaxLength - 1;
    internal const int PositionMask = 0x00FFFFFF;
    readonly int _packed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SepCharPosition(char character, int position)
    {
        _packed = PackRaw(character, position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SepCharPosition(byte character, int position)
    {
        _packed = PackRaw(character, position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal SepCharPosition(int packed)
    {
        _packed = packed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PackRaw(char character, int position)
    {
        A.Assert(0 < character && character < byte.MaxValue);
        A.Assert(0 <= position && position < (1 << CharShift));
        return character << 24 | position;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PackRaw(byte character, int position)
    {
        A.Assert(0 < character && character < byte.MaxValue);
        A.Assert(0 <= position && position < (1 << CharShift));
        return character << 24 | position;
    }

#if DEBUG
    public static Pos Pack(char character, int position) => new(character, position);
    public static Pos Pack(byte character, int position) => new(character, position);
    public static char UnpackCharacter(Pos pos) => (char)(pos._packed >> CharShift);
    public static int UnpackPosition(Pos pos) => pos._packed & PositionMask;
#else
    public static Pos Pack(char character, int position) => PackRaw(character, position);
    public static Pos Pack(byte character, int position) => PackRaw(character, position);
    public static int UnpackCharacter(Pos pos) => pos >> CharShift;
    public static int UnpackPosition(Pos pos) => pos & PositionMask;
#endif

    public int Packed => _packed;
    public char Character => (char)(_packed >> CharShift);
    public byte CharacterByte => (byte)(_packed >> CharShift);
    public int Position => _packed & PositionMask;

    public void Deconstruct(out char character, out int position)
    {
        character = Character;
        position = Position;
    }

    public static bool operator ==(SepCharPosition left, SepCharPosition right) => left.Equals(right);
    public static bool operator !=(SepCharPosition left, SepCharPosition right) => !(left == right);

    public bool Equals(SepCharPosition other) => _packed == other._packed;

    public override bool Equals(object? obj) => obj is SepCharPosition p && Equals(p);
    public override int GetHashCode() => _packed.GetHashCode();

    public override string ToString() => $"'{Character}'={CharacterByte}={Convert.ToString(CharacterByte, 2).PadLeft(8, '0')} at '{Position:D4}'";

    internal string DebuggerDisplay => ToString();
}
