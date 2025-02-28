using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using LinkDotNet.StringBuilder;

namespace BigFloatSharp;

/// <summary>
/// An arbitrary size and precision floating-point number stored as the quotient of two BigIntegers.
/// </summary>
[Serializable]
public readonly struct BigFloat : IComparable, IComparable<BigFloat>, IEquatable<BigFloat> {
    /// <summary>
    /// The dividend (top of the fraction).
    /// </summary>
    public readonly BigInteger Numerator;
    /// <summary>
    /// The divisor (bottom of the fraction).
    /// </summary>
    public readonly BigInteger Denominator;

    /// <summary>
    /// A value representing the number 1.
    /// </summary>
    public static BigFloat One { get; } = new(BigInteger.One);
    /// <summary>
    /// A value representing the number 0.
    /// </summary>
    public static BigFloat Zero { get; } = new(BigInteger.Zero);
    /// <summary>
    /// A value representing the number -1.
    /// </summary>
    public static BigFloat NegativeOne { get; } = new(BigInteger.MinusOne);
    /// <summary>
    /// A value representing the number 0.5.
    /// </summary>
    public static BigFloat OneHalf { get; } = new(BigInteger.One, 2);
    /// <summary>
    /// A value representing a value that is not a number.
    /// </summary>
    public static BigFloat NaN { get; } = new(0, 0);
    /// <summary>
    /// A value representing a value that is positive infinity.
    /// </summary>
    public static BigFloat PositiveInfinity { get; } = new(1, 0);
    /// <summary>
    /// A value representing a value that is negative infinity.
    /// </summary>
    public static BigFloat NegativeInfinity { get; } = new(-1, 0);

    /// <summary>
    /// Represents 100 digits of the natural logarithmic base, specified by the constant, e.
    /// </summary>
    public static BigFloat E { get; } = Parse("2.7182818284590452353602874713526624977572470936999595749669676277240766303535475945713821785251664274");
    /// <summary>
    /// Represents 100 digits of the ratio of the circumference of a circle to its diameter, specified by the constant, π.
    /// </summary>
    public static BigFloat Pi { get; } = Parse("3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679");
    /// <summary>
    /// Represents 100 digits of the number of radians in one turn, specified by the constant, τ.
    /// </summary>
    public static BigFloat Tau { get; } = Parse("6.2831853071795864769252867665590057683943387987502116419498891846156328125724179972560696506842341359");

    /// <summary>
    /// The number that, when added to a number, returns the other number.
    /// </summary>
    public static BigFloat AdditiveIdentity => Zero;
    /// <summary>
    /// The number that, when multiplied by a number, returns the other number.
    /// </summary>
    public static BigFloat MultiplicativeIdentity => One;

    /// <summary>
    /// Gets a number that indicates the sign of this number.
    /// <list type="bullet">
    /// <item>1 if positive</item>
    /// <item>0 if zero</item>
    /// <item>-1 if negative</item>
    /// </list>
    /// </summary>
    public int Sign => (Numerator.Sign + Denominator.Sign) switch {
        2 or -2 => 1,
        0 => -1,
        _ => 0,
    };

    #region Constructors

    /// <summary>
    /// Constructs a value of 0.
    /// </summary>
    public BigFloat() {
        (Numerator, Denominator) = (BigInteger.Zero, BigInteger.One);
    }
    /// <summary>
    /// Constructs a value from a numerator and denominator.
    /// </summary>
    public BigFloat(BigInteger numerator, BigInteger denominator) {
        (Numerator, Denominator) = (numerator, denominator);
    }
    /// <summary>
    /// Constructs a value from a <see cref="BigInteger"/>.
    /// </summary>
    public BigFloat(BigInteger value) {
        (Numerator, Denominator) = (value, BigInteger.One);
    }
    /// <summary>
    /// Constructs a value from a <see cref="BigFloat"/>.
    /// </summary>
    public BigFloat(BigFloat value) {
        (Numerator, Denominator) = value;
    }
    /// <summary>
    /// Constructs a value from an <see cref="int"/>.
    /// </summary>
    public BigFloat(int value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="uint"/>.
    /// </summary>
    public BigFloat(uint value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="long"/>.
    /// </summary>
    public BigFloat(long value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="ulong"/>.
    /// </summary>
    public BigFloat(ulong value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="float"/>.
    /// </summary>
    public BigFloat(float value) {
        if (value % 1 == 0) {
            (Numerator, Denominator) = ((BigInteger)value, 1);
        }
        else {
            (Numerator, Denominator) = Parse(value.ToString(CultureInfo.InvariantCulture));
        }
    }
    /// <summary>
    /// Constructs a value from a <see cref="double"/>.
    /// </summary>
    public BigFloat(double value) {
        if (value % 1 == 0) {
            (Numerator, Denominator) = ((BigInteger)value, 1);
        }
        else {
            (Numerator, Denominator) = Parse(value.ToString(CultureInfo.InvariantCulture));
        }
    }
    /// <summary>
    /// Constructs a value from a <see cref="decimal"/>.
    /// </summary>
    public BigFloat(decimal value) {
        if (value % 1 == 0) {
            (Numerator, Denominator) = ((BigInteger)value, 1);
        }
        else {
            (Numerator, Denominator) = Parse(value.ToString(CultureInfo.InvariantCulture));
        }
    }

    #endregion

    #region Static Methods

    public static BigFloat Add(BigFloat value, BigFloat other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        BigInteger numerator = (value.Numerator * other.Denominator) + (other.Numerator * value.Denominator);
        return new BigFloat(numerator, value.Denominator * other.Denominator);
    }
    public static BigFloat Subtract(BigFloat value, BigFloat other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        BigInteger numerator = (value.Numerator * other.Denominator) - (other.Numerator * value.Denominator);
        return new BigFloat(numerator, value.Denominator * other.Denominator);
    }
    public static BigFloat Multiply(BigFloat value, BigFloat other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        return new BigFloat(value.Numerator * other.Numerator, value.Denominator * other.Denominator);
    }
    public static BigFloat Divide(BigFloat value, BigFloat other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        return new BigFloat(value.Numerator * other.Denominator, value.Denominator * other.Numerator);
    }
    public static BigFloat Remainder(BigFloat value, BigFloat other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        return value - Floor(value / other) * other;
    }
    public static BigFloat DivRem(BigFloat value, BigFloat other, out BigFloat remainder) {
        value = Divide(value, other);
        remainder = Remainder(value, other);
        return value;
    }
    public static (BigFloat Quotient, BigFloat Remainder) DivRem(BigFloat value, BigFloat other) {
        BigFloat quotient = DivRem(value, other, out BigFloat remainder);
        return (quotient, remainder);
    }
    public static BigFloat Pow(BigFloat value, int exponent) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
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
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return new BigFloat(value.Numerator + value.Denominator, value.Denominator);
    }
    public static BigFloat Decrement(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return new BigFloat(value.Numerator - value.Denominator, value.Denominator);
    }
    public static BigFloat Ceil(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        BigInteger numerator = value.Numerator;
        if (numerator < 0) {
            numerator -= BigInteger.Remainder(numerator, value.Denominator);
        }
        else {
            numerator += value.Denominator - BigInteger.Remainder(numerator, value.Denominator);
        }
        return new BigFloat(numerator, value.Denominator);
    }
    public static BigFloat Floor(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        BigInteger numerator = value.Numerator;
        if (numerator < 0) {
            numerator += value.Denominator - BigInteger.Remainder(numerator, value.Denominator);
        }
        else {
            numerator -= BigInteger.Remainder(numerator, value.Denominator);
        }
        return new BigFloat(numerator, value.Denominator);
    }
    public static BigFloat Round(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (GetFractionalPart(value).CompareTo(OneHalf) >= 0) {
            return Ceil(value);
        }
        else {
            return Floor(value);
        }
    }
    public static BigFloat Round(BigFloat value, int digits, MidpointRounding mode) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        throw new NotImplementedException();
    }
    public static BigFloat Truncate(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return GetWholePart(value);
    }
    public static BigInteger GetWholePart(BigFloat value) {
        return value.Numerator / value.Denominator;
    }
    public static BigFloat GetFractionalPart(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return new BigFloat(BigInteger.Remainder(value.Numerator, value.Denominator), value.Denominator);
    }
    public static BigFloat ShiftLeft(BigFloat value, int shift = 1, int numberBase = 2) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (shift < 0) {
            return ShiftRight(value, -shift);
        }
        return new BigFloat(value.Numerator * BigInteger.Pow(numberBase, shift), value.Denominator);
    }
    public static BigFloat ShiftRight(BigFloat value, int shift = 1, int numberBase = 2) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (shift < 0) {
            return ShiftLeft(value, -shift);
        }
        return new BigFloat(value.Numerator, value.Denominator * BigInteger.Pow(numberBase, shift));
    }
    public static BigFloat Sqrt(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return Divide(Math.Pow(10, BigInteger.Log10(value.Numerator) / 2), Math.Pow(10, BigInteger.Log10(value.Denominator) / 2));
    }
    /// <summary>
    /// Simplifies the numerator and denominator to their canonical form.
    /// </summary>
    /// <remarks>
    /// Factorizing can be very slow, so use only when necessary.
    /// </remarks>
    public static BigFloat Factorize(BigFloat value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (value.Denominator.IsOne || value.Denominator.IsZero) {
            return value;
        }
        BigInteger factor = BigInteger.GreatestCommonDivisor(value.Numerator, value.Denominator);
        return new BigFloat(value.Numerator / factor, value.Denominator / factor);
    }
    /// <summary>
    /// Parses a value from the given string.
    /// </summary>
    public static BigFloat Parse(string value) {
        return Parse(value, null);
    }
    /// <summary>
    /// Parses a value from the given string.
    /// </summary>
    public static BigFloat Parse(string value, IFormatProvider? provider = null) {
        return Parse(value, NumberStyles.Float, provider);
    }
    /// <summary>
    /// Parses a value from the given string.
    /// </summary>
    public static BigFloat Parse(string value, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) {
        NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

        // Try parse named literal
        ReadOnlySpan<char> trimmedValue = value.AsSpan().Trim();
        // Infinity
        if (trimmedValue.Equals(numberFormat.PositiveInfinitySymbol, StringComparison.OrdinalIgnoreCase)) {
            return PositiveInfinity;
        }
        // -Infinity
        else if (trimmedValue.Equals(numberFormat.NegativeInfinitySymbol, StringComparison.OrdinalIgnoreCase)) {
            return NegativeInfinity;
        }
        // NaN
        else if (trimmedValue.Equals(numberFormat.NaNSymbol, StringComparison.OrdinalIgnoreCase)) {
            return NaN;
        }
        // -
        else if (trimmedValue.StartsWith(numberFormat.NegativeSign)) {
            ReadOnlySpan<char> trimmedValueAfterSign = trimmedValue[numberFormat.NegativeSign.Length..];
            // Infinity
            if (trimmedValueAfterSign.Equals(numberFormat.PositiveInfinitySymbol, StringComparison.OrdinalIgnoreCase)) {
                return NegativeInfinity;
            }
            // NaN
            else if (trimmedValueAfterSign.Equals(numberFormat.NaNSymbol, StringComparison.OrdinalIgnoreCase)) {
                return NaN;
            }
        }
        // +
        else if (trimmedValue.StartsWith(numberFormat.PositiveSign)) {
            ReadOnlySpan<char> trimmedValueAfterSign = trimmedValue[numberFormat.PositiveSign.Length..];
            // Infinity
            if (trimmedValueAfterSign.Equals(numberFormat.PositiveInfinitySymbol, StringComparison.OrdinalIgnoreCase)) {
                return PositiveInfinity;
            }
            // NaN
            else if (trimmedValueAfterSign.Equals(numberFormat.NaNSymbol, StringComparison.OrdinalIgnoreCase)) {
                return NaN;
            }
        }

        // Find decimal point
        int decimalPointPos = value.IndexOf(numberFormat.NumberDecimalSeparator);
        // No decimal point
        if (decimalPointPos < 0) {
            return Factorize(BigInteger.Parse(value, style, provider));
        }

        // Remove decimal point
        value = value.Replace(numberFormat.NumberDecimalSeparator, "");
        // Get numerator and denominator
        BigInteger numerator = BigInteger.Parse(value, style, provider);
        BigInteger denominator = BigInteger.Pow(10, value.Length - decimalPointPos);
        return new BigFloat(numerator, denominator);
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
    /// <summary>
    /// Returns whether the value is in its simplest form.
    /// </summary>
    public static bool IsCanonical(BigFloat value) {
        return BigInteger.GreatestCommonDivisor(value.Numerator, value.Denominator).IsOne;
    }
    /// <summary>
    /// Returns whether the value is divisible by 2.
    /// </summary>
    public static bool IsEven(BigFloat value) {
        return value % 2 == 0;
    }
    /// <summary>
    /// Returns whether the value is not divisible by 2.
    /// </summary>
    public static bool IsOdd(BigFloat value) {
        return value % 2 != 0;
    }
    /// <summary>
    /// Returns whether the value is a whole number.
    /// </summary>
    public static bool IsInteger(BigFloat value) {
        return BigInteger.Remainder(value.Numerator, value.Denominator).IsZero;
    }
    /// <summary>
    /// Returns whether the value is a whole number divisible by 2.
    /// </summary>
    public static bool IsEvenInteger(BigFloat value) {
        return IsEven(value) && IsInteger(value);
    }
    /// <summary>
    /// Returns whether the value is a whole number not divisible by 2.
    /// </summary>
    public static bool IsOddInteger(BigFloat value) {
        return IsOdd(value) && IsInteger(value);
    }
    /// <summary>
    /// Returns whether the value is not a number.
    /// </summary>
    public static bool IsNaN(BigFloat value) {
        return value.Numerator.IsZero && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value is finite (not infinity or NaN).
    /// </summary>
    public static bool IsFinite(BigFloat value) {
        return !value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value represents infinity.
    /// </summary>
    public static bool IsInfinity(BigFloat value) {
        return !value.Numerator.IsZero && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value represents positive infinity.
    /// </summary>
    public static bool IsPositiveInfinity(BigFloat value) {
        return value.Numerator > 0 && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value represents negative infinity.
    /// </summary>
    public static bool IsNegativeInfinity(BigFloat value) {
        return value.Numerator < 0 && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value is greater than zero.
    /// </summary>
    public static bool IsPositive(BigFloat value) {
        return (value.Numerator > 0) ^ (value.Denominator > 0);
    }
    /// <summary>
    /// Returns whether the value is less than zero.
    /// </summary>
    public static bool IsNegative(BigFloat value) {
        return (value.Numerator < 0) ^ (value.Denominator < 0);
    }
    /// <summary>
    /// Returns whether the value is zero.
    /// </summary>
    public static bool IsZero(BigFloat value) {
        return value.Numerator.IsZero;
    }
    /// <summary>
    /// Returns the greater of the two values.
    /// </summary>
    public static BigFloat Max(BigFloat a, BigFloat b) {
        return a > b ? a : b;
    }
    /// <summary>
    /// Returns the lesser of the two values.
    /// </summary>
    public static BigFloat Min(BigFloat a, BigFloat b) {
        return a < b ? a : b;
    }
    /// <summary>
    /// Returns the linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.
    /// </summary>
    public static BigFloat Lerp(BigFloat a, BigFloat b, BigFloat t) {
        return a + (b - a) * t;
    }
    /// <summary>
    /// Returns the inverse linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.
    /// </summary>
    public static BigFloat InverseLerp(BigFloat a, BigFloat b, BigFloat value) {
        return (value - a) / (b - a);
    }

    #endregion

    #region Instance Methods

    /// <summary>
    /// Stringifies the value as a decimal, truncating at 100 decimal places.
    /// </summary>
    public override string ToString() {
        return ToString(precision: 100);
    }
    /// <summary>
    /// Stringifies the value as a decimal, truncating at <paramref name="precision"/> decimal places.<br/>
    /// The number is optionally padded with <c>.0</c> if it's an integer.
    /// </summary>
    public string ToString(int precision, IFormatProvider? provider = null, bool padDecimal = false) {
        NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

        // Infinity
        if (IsPositiveInfinity(this)) {
            return numberFormat.PositiveInfinitySymbol;
        }
        // -Infinity
        else if (IsNegativeInfinity(this)) {
            return numberFormat.NegativeInfinitySymbol;
        }
        // NaN
        else if (IsNaN(this)) {
            return numberFormat.NaNSymbol;
        }

        // Get whole part (e.g. 123.45 -> 123)
        BigInteger whole = BigInteger.DivRem(Numerator, Denominator, out BigInteger remainder);
        string wholeString = whole.ToString(numberFormat);

        // Number is whole
        if (remainder.IsZero) {
            if (padDecimal) {
                return wholeString + numberFormat.NumberDecimalSeparator + "0";
            }
            return wholeString;
        }

        // Get decimal as scaled integer (e.g. 123.45 -> 1234500000)
        BigInteger fractional = (Numerator * BigInteger.Pow(10, precision)) / Denominator;

        // Get fraction part (e.g. 123.45 -> 4500000)
        using ValueStringBuilder fractionBuilder = new(stackalloc char[64]);
        for (int columnNumber = 0; columnNumber < precision; columnNumber++) {
            // Add column digit
            fractionBuilder.Append(fractional % 10);
            fractional /= 10;
        }
        fractionBuilder.Reverse();
        fractionBuilder.TrimEnd('0');
        string fractionString = fractionBuilder.ToString();

        // Combine parts
        return wholeString + numberFormat.NumberDecimalSeparator + fractionString;
    }
    /// <summary>
    /// Stringifies the value as a simplified rational (numerator / denominator).
    /// </summary>
    public string ToRationalString() {
        BigFloat value = Factorize(this);
        return value.Numerator + " / " + value.Denominator;
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
    /// <summary>
    /// Gets a hash code for the numerator and denominator of the value.
    /// </summary>
    public override int GetHashCode() {
        return HashCode.Combine(Numerator, Denominator);
    }
    /// <summary>
    /// Gets the numerator and denominator of the value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deconstruct(out BigInteger numerator, out BigInteger denominator) {
        (numerator, denominator) = (Numerator, Denominator);
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
    public static BigFloat operator >>(BigFloat value, int shift) => ShiftRight(value, shift);
    public static BigFloat operator <<(BigFloat value, int shift) => ShiftLeft(value, shift);
    public static BigFloat operator ~(BigFloat value) => Inverse(value);
    public static bool operator ==(BigFloat left, BigFloat right) => Compare(left, right) == 0;
    public static bool operator !=(BigFloat left, BigFloat right) => Compare(left, right) != 0;
    public static bool operator <(BigFloat left, BigFloat right) => Compare(left, right) < 0;
    public static bool operator <=(BigFloat left, BigFloat right) => Compare(left, right) <= 0;
    public static bool operator >(BigFloat left, BigFloat right) => Compare(left, right) > 0;
    public static bool operator >=(BigFloat left, BigFloat right) => Compare(left, right) >= 0;

    #endregion

    #region Casts

    public static explicit operator Half(BigFloat value) {
        if (value < Half.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than Half.MinValue.");
        }
        if (value > Half.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than Half.MaxValue.");
        }
        return (Half)value.Numerator / (Half)value.Denominator;
    }
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

    public static implicit operator BigFloat(sbyte value) => new(value);
    public static implicit operator BigFloat(byte value) => new((uint)value);
    public static implicit operator BigFloat(short value) => new(value);
    public static implicit operator BigFloat(ushort value) => new((uint)value);
    public static implicit operator BigFloat(int value) => new(value);
    public static implicit operator BigFloat(uint value) => new(value);
    public static implicit operator BigFloat(long value) => new(value);
    public static implicit operator BigFloat(ulong value) => new(value);
    public static implicit operator BigFloat(Int128 value) => new(value);
    public static implicit operator BigFloat(UInt128 value) => new(value);
    public static implicit operator BigFloat(Half value) => new((float)value);
    public static implicit operator BigFloat(float value) => new(value);
    public static implicit operator BigFloat(double value) => new(value);
    public static implicit operator BigFloat(decimal value) => new(value);
    public static implicit operator BigFloat(char value) => new(value);
    public static implicit operator BigFloat(nint value) => new((BigInteger)value);
    public static implicit operator BigFloat(nuint value) => new((BigInteger)value);
    public static implicit operator BigFloat(BigInteger value) => new(value);

    #endregion
}
