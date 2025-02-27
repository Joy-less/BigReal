using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LinkDotNet.StringBuilder;

namespace System.Numerics;

[Serializable]
public readonly partial struct BigFloat : IComparable, IComparable<BigFloat>, IEquatable<BigFloat>, IFloatingPoint<BigFloat> {
    public readonly BigInteger Numerator;
    public readonly BigInteger Denominator;

    public static BigFloat One { get; } = new(BigInteger.One);
    public static BigFloat Zero { get; } = new(BigInteger.Zero);
    public static BigFloat NegativeOne { get; } = new(BigInteger.MinusOne);
    public static BigFloat OneHalf { get; } = new(BigInteger.One, 2);

    public static BigFloat E { get; } = Parse("2.7182818284590452353602874713526624977572");
    public static BigFloat Pi { get; } = Parse("3.1415926535897932384626433832795028841971");
    public static BigFloat Tau { get; } = Parse("6.2831853071795864769252867665590057683943");

    public static int Radix => 2;
    public static BigFloat AdditiveIdentity => Zero;
    public static BigFloat MultiplicativeIdentity => One;

    public int Sign => (Numerator.Sign + Denominator.Sign) switch {
        2 or -2 => 1,
        0 => -1,
        _ => 0,
    };

    #region Constructors

    public BigFloat() {
        (Numerator, Denominator) = (BigInteger.Zero, BigInteger.One);
    }
    public BigFloat(BigInteger numerator, BigInteger denominator) {
        if (denominator == 0) {
            throw new DivideByZeroException(nameof(denominator));
        }
        (Numerator, Denominator) = (numerator, denominator);
    }
    public BigFloat(BigInteger value) {
        (Numerator, Denominator) = (value, BigInteger.One);
    }
    public BigFloat(BigFloat value) {
        (Numerator, Denominator) = value;
    }
    public BigFloat(int value)
        : this(new BigInteger(value)) {
    }
    public BigFloat(uint value)
        : this(new BigInteger(value)) {
    }
    public BigFloat(long value)
        : this(new BigInteger(value)) {
    }
    public BigFloat(ulong value)
        : this(new BigInteger(value)) {
    }
    public BigFloat(float value) {
        (Numerator, Denominator) = Parse(value.ToString("N99"));
    }
    public BigFloat(double value) {
        (Numerator, Denominator) = Parse(value.ToString("N99"));
    }
    public BigFloat(decimal value) {
        (Numerator, Denominator) = Parse(value.ToString("N99"));
    }

    #endregion

    #region Static Methods

    public static BigFloat Add(BigFloat value, BigFloat other) {
        BigInteger numerator = (value.Numerator * other.Denominator) + (other.Numerator * value.Denominator);
        return new BigFloat(numerator, value.Denominator * other.Denominator);
    }
    public static BigFloat Subtract(BigFloat value, BigFloat other) {
        BigInteger numerator = (value.Numerator * other.Denominator) - (other.Numerator * value.Denominator);
        return new BigFloat(numerator, value.Denominator * other.Denominator);
    }
    public static BigFloat Multiply(BigFloat value, BigFloat other) {
        return new BigFloat(value.Numerator * other.Numerator, value.Denominator * other.Denominator);
    }
    public static BigFloat Divide(BigFloat value, BigFloat other) {
        if (other.Numerator == 0) {
            throw new DivideByZeroException(nameof(other));
        }
        return new BigFloat(value.Numerator * other.Denominator, value.Denominator * other.Numerator);
    }
    public static BigFloat Remainder(BigFloat value, BigFloat other) {
        return value - Floor(value / other) * other;
    }
    public static BigFloat DivideRemainder(BigFloat value, BigFloat other, out BigFloat remainder) {
        value = Divide(value, other);
        remainder = Remainder(value, other);
        return value;
    }
    public static BigFloat Pow(BigFloat value, int exponent) {
        if (value.Numerator.IsZero) {
            return value;
        }
        else if (exponent < 0) {
            BigInteger savedNumerator = value.Numerator;
            BigInteger numerator = BigInteger.Pow(value.Denominator, -exponent);
            BigInteger denominator = BigInteger.Pow(savedNumerator, -exponent);
            return new(numerator, denominator);
        }
        else {
            BigInteger numerator = BigInteger.Pow(value.Numerator, exponent);
            BigInteger denominator = BigInteger.Pow(value.Denominator, exponent);
            return new(numerator, denominator);
        }
    }
    public static BigFloat Abs(BigFloat value) {
        return new BigFloat(BigInteger.Abs(value.Numerator), value.Denominator);
    }
    public static BigFloat Negate(BigFloat value) {
        return new BigFloat(BigInteger.Negate(value.Numerator), value.Denominator);
    }
    public static BigFloat Inverse(BigFloat value) {
        return new BigFloat(value.Denominator, value.Numerator);
    }
    public static BigFloat Increment(BigFloat value) {
        return new BigFloat(value.Numerator + value.Denominator, value.Denominator);
    }
    public static BigFloat Decrement(BigFloat value) {
        return new BigFloat(value.Numerator - value.Denominator, value.Denominator);
    }
    public static BigFloat Ceil(BigFloat value) {
        BigInteger numerator = value.Numerator;
        if (numerator < 0) {
            numerator -= BigInteger.Remainder(numerator, value.Denominator);
        }
        else {
            numerator += value.Denominator - BigInteger.Remainder(numerator, value.Denominator);
        }
        return Factor(new BigFloat(numerator, value.Denominator));
    }
    public static BigFloat Floor(BigFloat value) {
        BigInteger numerator = value.Numerator;
        if (numerator < 0) {
            numerator += value.Denominator - BigInteger.Remainder(numerator, value.Denominator);
        }
        else {
            numerator -= BigInteger.Remainder(numerator, value.Denominator);
        }
        return Factor(new(numerator, value.Denominator));
    }
    public static BigFloat Round(BigFloat value) {
        if (GetFractionalPart(value).CompareTo(OneHalf) >= 0) {
            return Ceil(value);
        }
        else {
            return Floor(value);
        }
    }
    public static BigFloat Round(BigFloat value, int digits, MidpointRounding mode) {
        throw new NotImplementedException();
    }
    public static BigFloat Truncate(BigFloat value) {
        BigInteger numerator = value.Numerator;
        numerator -= BigInteger.Remainder(numerator, value.Denominator);
        return Factor(new BigFloat(numerator, value.Denominator));
    }
    public static BigFloat GetWholePart(BigFloat value) {
        return Truncate(value);
    }
    public static BigFloat GetFractionalPart(BigFloat value) {
        return new BigFloat(BigInteger.Remainder(value.Numerator, value.Denominator), value.Denominator);
    }
    public static BigFloat ShiftDecimalLeft(BigFloat value, int shift) {
        if (shift < 0) {
            return ShiftDecimalRight(value, -shift);
        }
        BigInteger numerator = value.Numerator * BigInteger.Pow(10, shift);
        return new(numerator, value.Denominator);
    }
    public static BigFloat ShiftDecimalRight(BigFloat value, int shift) {
        if (shift < 0) {
            return ShiftDecimalLeft(value, -shift);
        }
        BigInteger denominator = value.Denominator * BigInteger.Pow(10, shift);
        return new(value.Numerator, denominator);
    }
    public static BigFloat Sqrt(BigFloat value) {
        return Divide(Math.Pow(10, BigInteger.Log10(value.Numerator) / 2), Math.Pow(10, BigInteger.Log10(value.Denominator) / 2));
    }
    public static double Log10(BigFloat value) {
        return BigInteger.Log10(value.Numerator) - BigInteger.Log10(value.Denominator);
    }
    public static double Log(BigFloat value, double baseValue) {
        return BigInteger.Log(value.Numerator, baseValue) - BigInteger.Log(value.Numerator, baseValue);
    }
    /// <remarks>
    /// Factoring can be very slow, so use only when necessary (<c>ToString</c> and comparisons).
    /// </remarks>
    public static BigFloat Factor(BigFloat value) {
        if (value.Denominator.IsOne) {
            return value;
        }
        BigInteger factor = BigInteger.GreatestCommonDivisor(value.Numerator, value.Denominator);
        return new BigFloat(value.Numerator / factor, value.Denominator / factor);
    }
    public static BigFloat Parse(string value) {
        return Parse(value.AsSpan());
    }
    public static BigFloat Parse(string value, IFormatProvider? provider = null) {
        return Parse(value, NumberStyles.Float, provider);
    }
    public static BigFloat Parse(string value, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) {
        NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

        value = value.Replace(numberFormat.NumberGroupSeparator, "");

        int decimalPointPos = value.IndexOf(numberFormat.NumberDecimalSeparator);
        value = value.Replace(numberFormat.NumberDecimalSeparator, "");

        // No decimal point
        if (decimalPointPos < 0) {
            return Factor(BigInteger.Parse(value, style, provider));
        }
        // Decimal point at (length - position - 1)
        else {
            BigInteger numerator = BigInteger.Parse(value, style, provider);
            BigInteger denominator = BigInteger.Pow(10, value.Length - decimalPointPos);
            return Factor(new BigFloat(numerator, denominator));
        }
    }
    public static BigFloat Parse(scoped ReadOnlySpan<char> value) {
        return Parse(value, null);
    }
    public static BigFloat Parse(scoped ReadOnlySpan<char> value, IFormatProvider? provider = null) {
        return Parse(value, NumberStyles.Float, provider);
    }
    public static BigFloat Parse(scoped ReadOnlySpan<char> value, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) {
        return Parse(value.ToString(), style, provider);
    }
    public static bool TryParse([NotNullWhen(true)] string? value, out BigFloat result) {
        return TryParse(value, null, out result);
    }
    public static bool TryParse([NotNullWhen(true)] string? value, IFormatProvider? provider, [MaybeNullWhen(false)] out BigFloat result) {
        return TryParse(value, NumberStyles.Float, provider, out result);
    }
    public static bool TryParse([NotNullWhen(true)] string? value, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out BigFloat result) {
        if (value is null) {
            result = default;
            return false;
        }
        try {
            result = Parse(value);
            return true;
        }
        catch (Exception) {
            result = default;
            return false;
        }
    }
    public static bool TryParse(ReadOnlySpan<char> value, [MaybeNullWhen(false)] out BigFloat result) {
        return TryParse(value, null, out result);
    }
    public static bool TryParse(ReadOnlySpan<char> value, IFormatProvider? provider, [MaybeNullWhen(false)] out BigFloat result) {
        return TryParse(value, NumberStyles.Float, provider, out result);
    }
    public static bool TryParse(ReadOnlySpan<char> value, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out BigFloat result) {
        return TryParse(value.ToString(), style, provider, out result);
    }
    public static int Compare(BigFloat left, BigFloat right) {
        return new BigFloat(left).CompareTo(right);
    }
    public static bool IsCanonical(BigFloat value) {
        return value.Denominator == 1 || value.Denominator == -1;
    }
    public static bool IsInteger(BigFloat value) {
        return value.Denominator == 1 || value.Denominator == -1;
    }
    public static bool IsEvenInteger(BigFloat value) {
        return (value.Denominator == 1 || value.Denominator == -1) && BigInteger.IsEvenInteger(value.Numerator);
    }
    public static bool IsOddInteger(BigFloat value) {
        return (value.Denominator == 1 || value.Denominator == -1) && BigInteger.IsOddInteger(value.Numerator);
    }
    public static bool IsComplexNumber(BigFloat value) {
        return false;
    }
    public static bool IsImaginaryNumber(BigFloat value) {
        return false;
    }
    public static bool IsRealNumber(BigFloat value) {
        return true;
    }
    public static bool IsNaN(BigFloat value) {
        return false;
    }
    public static bool IsFinite(BigFloat value) {
        return true;
    }
    public static bool IsInfinity(BigFloat value) {
        return false;
    }
    public static bool IsPositiveInfinity(BigFloat value) {
        return false;
    }
    public static bool IsNegativeInfinity(BigFloat value) {
        return false;
    }
    public static bool IsPositive(BigFloat value) {
        return (value.Numerator > 0) ^ (value.Denominator > 0);
    }
    public static bool IsNegative(BigFloat value) {
        return (value.Numerator < 0) ^ (value.Denominator < 0);
    }
    public static bool IsNormal(BigFloat value) {
        return !value.Numerator.IsZero;
    }
    public static bool IsSubnormal(BigFloat value) {
        return !value.Numerator.IsZero;
    }
    public static bool IsZero(BigFloat value) {
        return value.Numerator.IsZero;
    }

    public static BigFloat MaxMagnitude(BigFloat x, BigFloat y) => x > y ? x : y;
    public static BigFloat MaxMagnitudeNumber(BigFloat x, BigFloat y) => MaxMagnitude(x, y);
    public static BigFloat MinMagnitude(BigFloat x, BigFloat y) => x < y ? x : y;
    public static BigFloat MinMagnitudeNumber(BigFloat x, BigFloat y) => MinMagnitude(x, y);

    static bool INumberBase<BigFloat>.TryConvertFromChecked<TOther>(TOther value, out BigFloat result) {
        throw new NotImplementedException();
    }
    static bool INumberBase<BigFloat>.TryConvertFromSaturating<TOther>(TOther value, out BigFloat result) {
        throw new NotImplementedException();
    }
    static bool INumberBase<BigFloat>.TryConvertFromTruncating<TOther>(TOther value, out BigFloat result) {
        throw new NotImplementedException();
    }
    static bool INumberBase<BigFloat>.TryConvertToChecked<TOther>(BigFloat value, out TOther result) {
        throw new NotImplementedException();
    }
    static bool INumberBase<BigFloat>.TryConvertToSaturating<TOther>(BigFloat value, out TOther result) {
        throw new NotImplementedException();
    }
    static bool INumberBase<BigFloat>.TryConvertToTruncating<TOther>(BigFloat value, out TOther result) {
        throw new NotImplementedException();
    }

    #endregion

    #region Instance Methods

    public override string ToString() {
        return ToString(precision: 100);
    }
    public string ToString(string? format, IFormatProvider? provider) {
        return ToString(precision: 100, provider);
    }
    public string ToString(int precision, IFormatProvider? provider = null, bool trailingPointZero = false) {
        NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

        BigInteger result = BigInteger.DivRem(Numerator, Denominator, out BigInteger remainder);
        if (remainder.IsZero) {
            if (trailingPointZero) {
                return result + numberFormat.NumberDecimalSeparator + "0";
            }
            else {
                return result.ToString(numberFormat);
            }
        }

        BigInteger decimals = (Numerator * BigInteger.Pow(10, precision)) / Denominator;
        if (decimals.IsZero) {
            if (trailingPointZero) {
                return result + numberFormat.NumberDecimalSeparator + "0";
            }
            else {
                return result.ToString(numberFormat);
            }
        }
        
        using ValueStringBuilder stringBuilder = new(stackalloc char[64]);
        while (precision > 0) {
            stringBuilder.Append(decimals % 10);
            decimals /= 10;
            precision--;
        }
        stringBuilder.Reverse();
        ReadOnlySpan<char> decimalsString = stringBuilder.AsSpan();

        return string.Concat(result.ToString(), numberFormat.NumberDecimalSeparator, decimalsString).TrimEnd('0');
    }
    public string ToRationalString() {
        BigFloat value = Factor(this);
        return value.Numerator + " / " + value.Denominator;
    }
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) {
        string result = ToString(format.ToString(), provider);
        if (!result.TryCopyTo(destination)) {
            charsWritten = default;
            return false;
        }
        charsWritten = result.Length;
        return true;
    }
    public int CompareTo(BigFloat other) {
        // Make copies
        BigInteger one = Numerator;
        BigInteger two = other.Numerator;

        // Cross-multiply
        one *= other.Denominator;
        two *= Denominator;

        // Test
        return BigInteger.Compare(one, two);
    }
    public int CompareTo(object? other) {
        if (other is null) {
            return 1;
        }
        else if (other is BigFloat otherBigFloat) {
            return CompareTo(otherBigFloat);
        }
        else {
            throw new ArgumentException($"{nameof(other)} is not a {nameof(BigFloat)}");
        }
    }
    public bool Equals(BigFloat other) {
        return other.Numerator * Denominator == Numerator * other.Denominator;
    }
    public override bool Equals(object? other) {
        return other is BigFloat otherBigFloat && Equals(otherBigFloat);
    }
    public override int GetHashCode() {
        return HashCode.Combine(Numerator, Denominator);
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deconstruct(out BigInteger numerator, out BigInteger denominator) {
        (numerator, denominator) = (Numerator, Denominator);
    }

    int IFloatingPoint<BigFloat>.GetExponentByteCount() {
        throw new NotImplementedException();
    }
    int IFloatingPoint<BigFloat>.GetExponentShortestBitLength() {
        throw new NotImplementedException();
    }
    int IFloatingPoint<BigFloat>.GetSignificandBitLength() {
        throw new NotImplementedException();
    }
    int IFloatingPoint<BigFloat>.GetSignificandByteCount() {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigFloat>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigFloat>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigFloat>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigFloat>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }

    #endregion

    #region Operators

    public static BigFloat operator -(BigFloat value) => Negate(value);
    public static BigFloat operator -(BigFloat left, BigFloat right) => Subtract(left, right);
    public static BigFloat operator --(BigFloat value) => Decrement(value);
    public static BigFloat operator +(BigFloat value) => value;
    public static BigFloat operator +(BigFloat left, BigFloat right) => Add(left, right);
    public static BigFloat operator ++(BigFloat value) => Increment(value);
    public static BigFloat operator %(BigFloat left, BigFloat right) => Remainder(left, right);
    public static BigFloat operator *(BigFloat left, BigFloat right) => Multiply(left, right);
    public static BigFloat operator /(BigFloat left, BigFloat right) => Divide(left, right);
    public static BigFloat operator >>(BigFloat value, int shift) => ShiftDecimalRight(value, shift);
    public static BigFloat operator <<(BigFloat value, int shift) => ShiftDecimalLeft(value, shift);
    public static BigFloat operator ~(BigFloat value) => Inverse(value);
    public static bool operator ==(BigFloat left, BigFloat right) => Compare(left, right) == 0;
    public static bool operator !=(BigFloat left, BigFloat right) => Compare(left, right) != 0;
    public static bool operator <(BigFloat left, BigFloat right) => Compare(left, right) < 0;
    public static bool operator <=(BigFloat left, BigFloat right) => Compare(left, right) <= 0;
    public static bool operator >(BigFloat left, BigFloat right) => Compare(left, right) > 0;
    public static bool operator >=(BigFloat left, BigFloat right) => Compare(left, right) >= 0;

    #endregion

    #region Casts

    public static explicit operator float(BigFloat value) {
        if (value < float.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than float.MinValue.");
        }
        if (value > float.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than float.MaxValue.");
        }
        return (float)value.Numerator / (float)value.Denominator;
    }
    public static explicit operator double(BigFloat value) {
        if (value < double.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than double.MinValue.");
        }
        if (value > double.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than double.MaxValue.");
        }
        return (double)value.Numerator / (double)value.Denominator;
    }
    public static explicit operator decimal(BigFloat value) {
        if (value < decimal.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than decimal.MinValue.");
        }
        if (value > decimal.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than decimal.MaxValue.");
        }
        return (decimal)value.Numerator / (decimal)value.Denominator;
    }

    public static implicit operator BigFloat(byte value) => new((BigInteger)value);
    public static implicit operator BigFloat(sbyte value) => new((BigInteger)value);
    public static implicit operator BigFloat(short value) => new((BigInteger)value);
    public static implicit operator BigFloat(ushort value) => new((BigInteger)value);
    public static implicit operator BigFloat(int value) => new((BigInteger)value);
    public static implicit operator BigFloat(uint value) => new((BigInteger)value);
    public static implicit operator BigFloat(long value) => new((BigInteger)value);
    public static implicit operator BigFloat(ulong value) => new((BigInteger)value);
    public static implicit operator BigFloat(Int128 value) => new((BigInteger)value);
    public static implicit operator BigFloat(UInt128 value) => new((BigInteger)value);
    public static implicit operator BigFloat(Half value) => new((BigInteger)value);
    public static implicit operator BigFloat(float value) => new((BigInteger)value);
    public static implicit operator BigFloat(double value) => new((BigInteger)value);
    public static implicit operator BigFloat(decimal value) => new((BigInteger)value);
    public static implicit operator BigFloat(char value) => new((BigInteger)value);
    public static implicit operator BigFloat(nint value) => new((BigInteger)value);
    public static implicit operator BigFloat(nuint value) => new((BigInteger)value);
    public static implicit operator BigFloat(BigInteger value) => new(value);

    #endregion
}
