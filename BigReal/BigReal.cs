using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using LinkDotNet.StringBuilder;

namespace ExtendedNumerics;

/// <summary>
/// An arbitrary size and precision floating-point number stored as the quotient of two BigIntegers.
/// </summary>
[Serializable]
public readonly struct BigReal : IComparable, IComparable<BigReal>, IEquatable<BigReal> {
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
    public static BigReal One { get; } = new(BigInteger.One);
    /// <summary>
    /// A value representing the number 0.
    /// </summary>
    public static BigReal Zero { get; } = new(BigInteger.Zero);
    /// <summary>
    /// A value representing the number -1.
    /// </summary>
    public static BigReal NegativeOne { get; } = new(BigInteger.MinusOne);
    /// <summary>
    /// A value representing the number 0.5.
    /// </summary>
    public static BigReal OneHalf { get; } = new(BigInteger.One, 2);
    /// <summary>
    /// A value representing a value that is not a number.
    /// </summary>
    public static BigReal NaN { get; } = new(0, 0);
    /// <summary>
    /// A value representing a value that is positive infinity.
    /// </summary>
    public static BigReal PositiveInfinity { get; } = new(1, 0);
    /// <summary>
    /// A value representing a value that is negative infinity.
    /// </summary>
    public static BigReal NegativeInfinity { get; } = new(-1, 0);

    /// <summary>
    /// Represents 100 digits of the natural logarithmic base, specified by the constant, e.
    /// </summary>
    public static BigReal E { get; } = Parse("2.7182818284590452353602874713526624977572470936999595749669676277240766303535475945713821785251664274");
    /// <summary>
    /// Represents 100 digits of the ratio of the circumference of a circle to its diameter, specified by the constant, π.
    /// </summary>
    public static BigReal Pi { get; } = Parse("3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679");
    /// <summary>
    /// Represents 100 digits of the number of radians in one turn, specified by the constant, τ.
    /// </summary>
    public static BigReal Tau { get; } = Parse("6.2831853071795864769252867665590057683943387987502116419498891846156328125724179972560696506842341359");

    /// <summary>
    /// The number that, when added to a number, returns the other number.
    /// </summary>
    public static BigReal AdditiveIdentity => Zero;
    /// <summary>
    /// The number that, when multiplied by a number, returns the other number.
    /// </summary>
    public static BigReal MultiplicativeIdentity => One;

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
    public BigReal() {
        (Numerator, Denominator) = (BigInteger.Zero, BigInteger.One);
    }
    /// <summary>
    /// Constructs a value from a numerator and denominator.
    /// </summary>
    public BigReal(BigInteger numerator, BigInteger denominator) {
        (Numerator, Denominator) = (numerator, denominator);
    }
    /// <summary>
    /// Constructs a value from a <see cref="BigInteger"/>.
    /// </summary>
    public BigReal(BigInteger value) {
        (Numerator, Denominator) = (value, BigInteger.One);
    }
    /// <summary>
    /// Constructs a value from a <see cref="BigReal"/>.
    /// </summary>
    public BigReal(BigReal value) {
        (Numerator, Denominator) = value;
    }
    /// <summary>
    /// Constructs a value from an <see cref="int"/>.
    /// </summary>
    public BigReal(int value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="uint"/>.
    /// </summary>
    public BigReal(uint value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="long"/>.
    /// </summary>
    public BigReal(long value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="ulong"/>.
    /// </summary>
    public BigReal(ulong value)
        : this(new BigInteger(value)) {
    }
    /// <summary>
    /// Constructs a value from a <see cref="float"/>.
    /// </summary>
    public BigReal(float value) {
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
    public BigReal(double value) {
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
    public BigReal(decimal value) {
        if (value % 1 == 0) {
            (Numerator, Denominator) = ((BigInteger)value, 1);
        }
        else {
            (Numerator, Denominator) = Parse(value.ToString(CultureInfo.InvariantCulture));
        }
    }

    #endregion

    #region Static Methods

    public static BigReal Add(BigReal value, BigReal other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        BigInteger numerator = (value.Numerator * other.Denominator) + (other.Numerator * value.Denominator);
        return new BigReal(numerator, value.Denominator * other.Denominator);
    }
    public static BigReal Subtract(BigReal value, BigReal other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        BigInteger numerator = (value.Numerator * other.Denominator) - (other.Numerator * value.Denominator);
        return new BigReal(numerator, value.Denominator * other.Denominator);
    }
    public static BigReal Multiply(BigReal value, BigReal other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        return new BigReal(value.Numerator * other.Numerator, value.Denominator * other.Denominator);
    }
    public static BigReal Divide(BigReal value, BigReal other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        return new BigReal(value.Numerator * other.Denominator, value.Denominator * other.Numerator);
    }
    public static BigReal Remainder(BigReal value, BigReal other) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (IsInfinity(other) || IsNaN(other)) {
            return value;
        }
        return value - Floor(value / other) * other;
    }
    public static BigReal DivRem(BigReal value, BigReal other, out BigReal remainder) {
        value = Divide(value, other);
        remainder = Remainder(value, other);
        return value;
    }
    public static (BigReal Quotient, BigReal Remainder) DivRem(BigReal value, BigReal other) {
        BigReal quotient = DivRem(value, other, out BigReal remainder);
        return (quotient, remainder);
    }
    public static BigReal Pow(BigReal value, int exponent) {
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
    public static BigReal Abs(BigReal value) {
        return new BigReal(BigInteger.Abs(value.Numerator), value.Denominator);
    }
    public static BigReal Negate(BigReal value) {
        return new BigReal(BigInteger.Negate(value.Numerator), value.Denominator);
    }
    public static BigReal Inverse(BigReal value) {
        return new BigReal(value.Denominator, value.Numerator);
    }
    public static BigReal Increment(BigReal value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return new BigReal(value.Numerator + value.Denominator, value.Denominator);
    }
    public static BigReal Decrement(BigReal value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return new BigReal(value.Numerator - value.Denominator, value.Denominator);
    }
    public static BigReal Ceil(BigReal value) {
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
        return new BigReal(numerator, value.Denominator);
    }
    public static BigReal Floor(BigReal value) {
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
        return new BigReal(numerator, value.Denominator);
    }
    public static BigReal Round(BigReal value) {
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
    public static BigReal Round(BigReal value, int digits, MidpointRounding mode) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        throw new NotImplementedException();
    }
    public static BigReal Truncate(BigReal value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return GetWholePart(value);
    }
    public static BigInteger GetWholePart(BigReal value) {
        return value.Numerator / value.Denominator;
    }
    public static BigReal GetFractionalPart(BigReal value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        return new BigReal(BigInteger.Remainder(value.Numerator, value.Denominator), value.Denominator);
    }
    public static BigReal ShiftLeft(BigReal value, int shift = 1, int numberBase = 2) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (shift < 0) {
            return ShiftRight(value, -shift);
        }
        return new BigReal(value.Numerator * BigInteger.Pow(numberBase, shift), value.Denominator);
    }
    public static BigReal ShiftRight(BigReal value, int shift = 1, int numberBase = 2) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (shift < 0) {
            return ShiftLeft(value, -shift);
        }
        return new BigReal(value.Numerator, value.Denominator * BigInteger.Pow(numberBase, shift));
    }
    public static BigReal Sqrt(BigReal value) {
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
    public static BigReal Factorize(BigReal value) {
        if (IsInfinity(value) || IsNaN(value)) {
            return value;
        }
        if (value.Denominator.IsOne || value.Denominator.IsZero) {
            return value;
        }
        BigInteger factor = BigInteger.GreatestCommonDivisor(value.Numerator, value.Denominator);
        return new BigReal(value.Numerator / factor, value.Denominator / factor);
    }
    /// <summary>
    /// Parses a value from the given string.
    /// </summary>
    public static BigReal Parse(string value) {
        return Parse(value, null);
    }
    /// <summary>
    /// Parses a value from the given string.
    /// </summary>
    public static BigReal Parse(string value, IFormatProvider? provider = null) {
        return Parse(value, NumberStyles.Float, provider);
    }
    /// <summary>
    /// Parses a value from the given string.
    /// </summary>
    public static BigReal Parse(string value, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) {
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
        return new BigReal(numerator, denominator);
    }
    public static BigReal Parse(scoped ReadOnlySpan<char> value) {
        return Parse(value, null);
    }
    public static BigReal Parse(scoped ReadOnlySpan<char> value, IFormatProvider? provider = null) {
        return Parse(value, NumberStyles.Float, provider);
    }
    public static BigReal Parse(scoped ReadOnlySpan<char> value, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) {
        return Parse(value.ToString(), style, provider);
    }
    public static bool TryParse([NotNullWhen(true)] string? value, out BigReal result) {
        return TryParse(value, null, out result);
    }
    public static bool TryParse([NotNullWhen(true)] string? value, IFormatProvider? provider, [MaybeNullWhen(false)] out BigReal result) {
        return TryParse(value, NumberStyles.Float, provider, out result);
    }
    public static bool TryParse([NotNullWhen(true)] string? value, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out BigReal result) {
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
    public static bool TryParse(ReadOnlySpan<char> value, [MaybeNullWhen(false)] out BigReal result) {
        return TryParse(value, null, out result);
    }
    public static bool TryParse(ReadOnlySpan<char> value, IFormatProvider? provider, [MaybeNullWhen(false)] out BigReal result) {
        return TryParse(value, NumberStyles.Float, provider, out result);
    }
    public static bool TryParse(ReadOnlySpan<char> value, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out BigReal result) {
        return TryParse(value.ToString(), style, provider, out result);
    }
    public static int Compare(BigReal left, BigReal right) {
        return new BigReal(left).CompareTo(right);
    }
    /// <summary>
    /// Returns whether the value is in its simplest form.
    /// </summary>
    public static bool IsCanonical(BigReal value) {
        return BigInteger.GreatestCommonDivisor(value.Numerator, value.Denominator).IsOne;
    }
    /// <summary>
    /// Returns whether the value is divisible by 2.
    /// </summary>
    public static bool IsEven(BigReal value) {
        return value % 2 == 0;
    }
    /// <summary>
    /// Returns whether the value is not divisible by 2.
    /// </summary>
    public static bool IsOdd(BigReal value) {
        return value % 2 != 0;
    }
    /// <summary>
    /// Returns whether the value is a whole number.
    /// </summary>
    public static bool IsInteger(BigReal value) {
        return BigInteger.Remainder(value.Numerator, value.Denominator).IsZero;
    }
    /// <summary>
    /// Returns whether the value is a whole number divisible by 2.
    /// </summary>
    public static bool IsEvenInteger(BigReal value) {
        return IsEven(value) && IsInteger(value);
    }
    /// <summary>
    /// Returns whether the value is a whole number not divisible by 2.
    /// </summary>
    public static bool IsOddInteger(BigReal value) {
        return IsOdd(value) && IsInteger(value);
    }
    /// <summary>
    /// Returns whether the value is not a number.
    /// </summary>
    public static bool IsNaN(BigReal value) {
        return value.Numerator.IsZero && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value is finite (not infinity or NaN).
    /// </summary>
    public static bool IsFinite(BigReal value) {
        return !value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value represents infinity.
    /// </summary>
    public static bool IsInfinity(BigReal value) {
        return !value.Numerator.IsZero && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value represents positive infinity.
    /// </summary>
    public static bool IsPositiveInfinity(BigReal value) {
        return value.Numerator > 0 && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value represents negative infinity.
    /// </summary>
    public static bool IsNegativeInfinity(BigReal value) {
        return value.Numerator < 0 && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether the value is greater than zero.
    /// </summary>
    public static bool IsPositive(BigReal value) {
        return (value.Numerator > 0) ^ (value.Denominator > 0);
    }
    /// <summary>
    /// Returns whether the value is less than zero.
    /// </summary>
    public static bool IsNegative(BigReal value) {
        return (value.Numerator < 0) ^ (value.Denominator < 0);
    }
    /// <summary>
    /// Returns whether the value is zero.
    /// </summary>
    public static bool IsZero(BigReal value) {
        return value.Numerator.IsZero;
    }
    /// <summary>
    /// Returns the greater of the two values.
    /// </summary>
    public static BigReal Max(BigReal a, BigReal b) {
        return a > b ? a : b;
    }
    /// <summary>
    /// Returns the lesser of the two values.
    /// </summary>
    public static BigReal Min(BigReal a, BigReal b) {
        return a < b ? a : b;
    }
    /// <summary>
    /// Returns the linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.
    /// </summary>
    public static BigReal Lerp(BigReal a, BigReal b, BigReal t) {
        return a + (b - a) * t;
    }
    /// <summary>
    /// Returns the inverse linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.
    /// </summary>
    public static BigReal InverseLerp(BigReal a, BigReal b, BigReal value) {
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
        BigReal value = Factorize(this);
        return value.Numerator + " / " + value.Denominator;
    }
    public int CompareTo(BigReal other) {
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
        else if (other is BigReal otherBigFloat) {
            return CompareTo(otherBigFloat);
        }
        else {
            throw new ArgumentException($"{nameof(other)} is not a {nameof(BigReal)}");
        }
    }
    public bool Equals(BigReal other) {
        return other.Numerator * Denominator == Numerator * other.Denominator;
    }
    public override bool Equals(object? other) {
        return other is BigReal otherBigFloat && Equals(otherBigFloat);
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

    public static BigReal operator -(BigReal value) => Negate(value);
    public static BigReal operator -(BigReal left, BigReal right) => Subtract(left, right);
    public static BigReal operator --(BigReal value) => Decrement(value);
    public static BigReal operator +(BigReal value) => value;
    public static BigReal operator +(BigReal left, BigReal right) => Add(left, right);
    public static BigReal operator ++(BigReal value) => Increment(value);
    public static BigReal operator %(BigReal left, BigReal right) => Remainder(left, right);
    public static BigReal operator *(BigReal left, BigReal right) => Multiply(left, right);
    public static BigReal operator /(BigReal left, BigReal right) => Divide(left, right);
    public static BigReal operator >>(BigReal value, int shift) => ShiftRight(value, shift);
    public static BigReal operator <<(BigReal value, int shift) => ShiftLeft(value, shift);
    public static BigReal operator ~(BigReal value) => Inverse(value);
    public static bool operator ==(BigReal left, BigReal right) => Compare(left, right) == 0;
    public static bool operator !=(BigReal left, BigReal right) => Compare(left, right) != 0;
    public static bool operator <(BigReal left, BigReal right) => Compare(left, right) < 0;
    public static bool operator <=(BigReal left, BigReal right) => Compare(left, right) <= 0;
    public static bool operator >(BigReal left, BigReal right) => Compare(left, right) > 0;
    public static bool operator >=(BigReal left, BigReal right) => Compare(left, right) >= 0;

    #endregion

    #region Casts

    public static explicit operator Half(BigReal value) {
        if (value < Half.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than Half.MinValue.");
        }
        if (value > Half.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than Half.MaxValue.");
        }
        return (Half)value.Numerator / (Half)value.Denominator;
    }
    public static explicit operator float(BigReal value) {
        if (value < float.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than float.MinValue.");
        }
        if (value > float.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than float.MaxValue.");
        }
        return (float)value.Numerator / (float)value.Denominator;
    }
    public static explicit operator double(BigReal value) {
        if (value < double.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than double.MinValue.");
        }
        if (value > double.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than double.MaxValue.");
        }
        return (double)value.Numerator / (double)value.Denominator;
    }
    public static explicit operator decimal(BigReal value) {
        if (value < decimal.MinValue) {
            throw new OverflowException($"{nameof(value)} is less than decimal.MinValue.");
        }
        if (value > decimal.MaxValue) {
            throw new OverflowException($"{nameof(value)} is greater than decimal.MaxValue.");
        }
        return (decimal)value.Numerator / (decimal)value.Denominator;
    }

    public static implicit operator BigReal(sbyte value) => new(value);
    public static implicit operator BigReal(byte value) => new((uint)value);
    public static implicit operator BigReal(short value) => new(value);
    public static implicit operator BigReal(ushort value) => new((uint)value);
    public static implicit operator BigReal(int value) => new(value);
    public static implicit operator BigReal(uint value) => new(value);
    public static implicit operator BigReal(long value) => new(value);
    public static implicit operator BigReal(ulong value) => new(value);
    public static implicit operator BigReal(Int128 value) => new(value);
    public static implicit operator BigReal(UInt128 value) => new(value);
    public static implicit operator BigReal(Half value) => new((float)value);
    public static implicit operator BigReal(float value) => new(value);
    public static implicit operator BigReal(double value) => new(value);
    public static implicit operator BigReal(decimal value) => new(value);
    public static implicit operator BigReal(char value) => new(value);
    public static implicit operator BigReal(nint value) => new((BigInteger)value);
    public static implicit operator BigReal(nuint value) => new((BigInteger)value);
    public static implicit operator BigReal(BigInteger value) => new(value);

    #endregion
}