using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;
using LinkDotNet.StringBuilder;

namespace ExtendedNumerics;

/// <summary>
/// An arbitrary size and precision floating-point number stored as the quotient of two BigIntegers.
/// </summary>
[Serializable]
public readonly partial struct BigReal : IConvertible, IComparable, IComparable<BigReal>, IEquatable<BigReal>, INumber<BigReal>, IFloatingPoint<BigReal>, IFloatingPointConstants<BigReal>,
    IPowerFunctions<BigReal>, IRootFunctions<BigReal>, ILogarithmicFunctions<BigReal>
{
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
    public static BigReal One { get; } = new(1);
    /// <summary>
    /// A value representing the number 10.
    /// </summary>
    public static BigReal Ten { get; } = new(10);
    /// <summary>
    /// A value representing the number 0.
    /// </summary>
    public static BigReal Zero { get; } = new(0);
    /// <summary>
    /// A value representing the number -1.
    /// </summary>
    public static BigReal NegativeOne { get; } = new(-1);
    /// <summary>
    /// A value representing the number 0.5.
    /// </summary>
    public static BigReal OneHalf { get; } = new(1, 2);
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
    /// The number that, when added to a number, returns the other number.
    /// </summary>
    public static BigReal AdditiveIdentity => Zero;
    /// <summary>
    /// The number that, when multiplied by a number, returns the other number.
    /// </summary>
    public static BigReal MultiplicativeIdentity => One;

    /// <summary>
    /// Returns:
    /// <list type="bullet">
    ///   <item>1 if the value is greater than zero</item>
    ///   <item>0 if the value is zero</item>
    ///   <item>-1 if the value is less than zero</item>
    /// </list>
    /// </summary>
    public int Sign => (Numerator.Sign + Denominator.Sign) switch {
        >= 2 or <= -2 => 1,
        0 => -1,
        _ => 0,
    };

    #region Constructors

    /// <summary>
    /// Constructs <see cref="BigReal"/> from 0.
    /// </summary>
    public BigReal()
        : this(BigInteger.Zero, BigInteger.One) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from a numerator and denominator.
    /// </summary>
    public BigReal(BigInteger numerator, BigInteger denominator) {
        (Numerator, Denominator) = (numerator, denominator);
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="BigInteger"/>.
    /// </summary>
    public BigReal(BigInteger value)
        : this(value, BigInteger.One) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="sbyte"/>.
    /// </summary>
    public BigReal(sbyte value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="byte"/>.
    /// </summary>
    public BigReal(byte value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="short"/>.
    /// </summary>
    public BigReal(short value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="ushort"/>.
    /// </summary>
    public BigReal(ushort value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="int"/>.
    /// </summary>
    public BigReal(int value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="uint"/>.
    /// </summary>
    public BigReal(uint value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="long"/>.
    /// </summary>
    public BigReal(long value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="ulong"/>.
    /// </summary>
    public BigReal(ulong value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="Int128"/>.
    /// </summary>
    public BigReal(Int128 value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="UInt128"/>.
    /// </summary>
    public BigReal(UInt128 value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="char"/>.
    /// </summary>
    public BigReal(char value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="Rune"/>.
    /// </summary>
    public BigReal(Rune value)
        : this((BigInteger)value.Value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="nint"/>.
    /// </summary>
    public BigReal(nint value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="nuint"/>.
    /// </summary>
    public BigReal(nuint value)
        : this((BigInteger)value) {
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="Half"/>.
    /// </summary>
    public BigReal(Half value) {
        if (Half.IsInteger(value)) {
            (Numerator, Denominator) = ((BigInteger)value, BigInteger.One);
        }
        else {
            this = new HalfHelper(value).ToBigReal();
        }
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="float"/>.
    /// </summary>
    public BigReal(float value) {
        if (float.IsInteger(value)) {
            (Numerator, Denominator) = ((BigInteger)value, BigInteger.One);
        }
        else {
            this = new SingleHelper(value).ToBigReal();
        }
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="double"/>.
    /// </summary>
    public BigReal(double value) {
        if (double.IsInteger(value)) {
            (Numerator, Denominator) = ((BigInteger)value, BigInteger.One);
        }
        else {
            this = new DoubleHelper(value).ToBigReal();
        }
    }
    /// <summary>
    /// Constructs <see cref="BigReal"/> from <see cref="decimal"/>.
    /// </summary>
    public BigReal(decimal value) {
        if (decimal.IsInteger(value)) {
            (Numerator, Denominator) = ((BigInteger)value, BigInteger.One);
        }
        else {
            this = new DecimalHelper(value).ToBigReal();
        }
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Returns the result of adding <paramref name="value"/> and <paramref name="other"/>.
    /// </summary>
    public static BigReal Add(BigReal value, BigReal other) {
        if (!IsFinite(value)) {
            return value;
        }
        if (!IsFinite(other)) {
            return other;
        }
        BigInteger numerator = (value.Numerator * other.Denominator) + (other.Numerator * value.Denominator);
        return new BigReal(numerator, value.Denominator * other.Denominator);
    }
    /// <summary>
    /// Returns the result of subtracting <paramref name="value"/> and <paramref name="other"/>.
    /// </summary>
    public static BigReal Subtract(BigReal value, BigReal other) {
        if (!IsFinite(value)) {
            return value;
        }
        if (!IsFinite(other)) {
            return other;
        }
        BigInteger numerator = (value.Numerator * other.Denominator) - (other.Numerator * value.Denominator);
        return new BigReal(numerator, value.Denominator * other.Denominator);
    }
    /// <summary>
    /// Returns the result of multiplying <paramref name="value"/> and <paramref name="other"/>.
    /// </summary>
    public static BigReal Multiply(BigReal value, BigReal other) {
        if (!IsFinite(value)) {
            return value;
        }
        if (!IsFinite(other)) {
            return other;
        }
        return new BigReal(value.Numerator * other.Numerator, value.Denominator * other.Denominator);
    }
    /// <summary>
    /// Returns the result of dividing <paramref name="value"/> and <paramref name="other"/>.
    /// </summary>
    public static BigReal Divide(BigReal value, BigReal other) {
        if (!IsFinite(value)) {
            return value;
        }
        if (!IsFinite(other)) {
            return other;
        }
        return new BigReal(value.Numerator * other.Denominator, value.Denominator * other.Numerator);
    }
    /// <summary>
    /// Returns the remainder of integer-dividing <paramref name="value"/> and <paramref name="other"/>.<br/>
    /// This is also called modulo.
    /// </summary>
    public static BigReal Remainder(BigReal value, BigReal other) {
        if (!IsFinite(value)) {
            return value;
        }
        if (!IsFinite(other)) {
            return other;
        }
        // https://github.com/AdamWhiteHat/BigRational/blob/aaacc836b15f415d070ecff7a37f66c4d9a94076/BigRational/Fraction.cs#L380
        return new BigReal(
            BigInteger.Multiply(value.Numerator, other.Denominator) % BigInteger.Multiply(value.Denominator, other.Numerator),
            BigInteger.Multiply(value.Denominator, other.Denominator)
        );
    }
    /// <summary>
    /// Returns the result of dividing <paramref name="value"/> and <paramref name="other"/> and the remainder of integer-dividing them.
    /// </summary>
    public static BigReal DivRem(BigReal value, BigReal other, out BigReal remainder) {
        BigReal quotient = Divide(value, other);
        remainder = Remainder(value, other);
        return quotient;
    }
    /// <summary>
    /// Returns the result of dividing <paramref name="value"/> and <paramref name="other"/> and the remainder of integer-dividing them.
    /// </summary>
    public static (BigReal Quotient, BigReal Remainder) DivRem(BigReal value, BigReal other) {
        BigReal quotient = DivRem(value, other, out BigReal remainder);
        return (quotient, remainder);
    }
    /// <summary>
    /// Returns <paramref name="value"/> to the power of <paramref name="exponent"/>.
    /// </summary>
    /// <remarks>
    /// See <see href="https://stackoverflow.com/a/30225002"/> for why <paramref name="exponent"/> is <see cref="int"/> rather than <see cref="BigInteger"/>.
    /// </remarks>
    public static BigReal Pow(BigReal value, int exponent) {
        if (!IsFinite(value)) {
            return value;
        }
        if (IsZero(value)) {
            return value;
        }
        switch (exponent) {
            case 0: {
                return One;
            }
            case 1: {
                return value;
            }
            case 2: {
                return value * value;
            }
            case 3: {
                return value * value * value;
            }
            case -1: {
                return new BigReal(value.Denominator, value.Numerator);
            }
            case -2: {
                BigReal flipped = new(value.Denominator, value.Numerator);
                return flipped * flipped;
            }
            case -3: {
                BigReal flipped = new(value.Denominator, value.Numerator);
                return flipped * flipped * flipped;
            }
            case > 0: {
                return new BigReal(
                    BigInteger.Pow(value.Numerator, exponent),
                    BigInteger.Pow(value.Denominator, exponent)
                );
            }
            case < 0: {
                return new BigReal(
                    BigInteger.Pow(value.Denominator, -exponent),
                    BigInteger.Pow(value.Numerator, -exponent)
                );
            }
        }
    }
    /// <summary>
    /// Returns <paramref name="value"/> to the power of <paramref name="exponent"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Pow(BigReal value, BigReal exponent, int decimals = DoubleReliableDecimals) {
        if (IsInteger(exponent)) {
            return Pow(value, (int)exponent);
        }
        if (TryCalculateAsDouble(value, exponent, double.Pow, decimals, out double result)) {
            return result;
        }

        /*
         * // Slow alternative:
         * // pow(value, numerator/denominator) = root(pow(value, numerator), denominator)
         * 
         * value = Simplify(value);
         * exponent = Simplify(exponent);
         * int power = (int)exponent.Numerator;
         * int root = (int)exponent.Denominator;
         * return RootN(Pow(value, power), root, decimals);
         */

        // Use the identity x^y = e^(y * ln(x))
        return Exp(exponent * Log(value, decimals: decimals), decimals: decimals);
    }
    static BigReal IPowerFunctions<BigReal>.Pow(BigReal value, BigReal exponent) {
        return Pow(value, exponent);
    }
    /// <summary>
    /// Returns the base e (natural) logarithm of <paramref name="value"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Log(BigReal value, int decimals = DoubleReliableDecimals) {
        if (TryCalculateAsDouble(value, double.Log, decimals, out double result)) {
            return result;
        }

        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/35971996
        BigReal oldSum = Zero;
        BigReal xmlxpl = (value - One) / (value + One);
        BigReal xmlxpl2 = xmlxpl * xmlxpl;
        BigReal denom = One;
        BigReal frac = xmlxpl;
        BigReal term = frac;
        BigReal sum = term;

        while (true) {
            oldSum = sum;
            denom += 2;
            frac *= xmlxpl2;
            sum += frac / denom;
            if (Abs(sum - oldSum) <= epsilon) {
                break;
            }
        }
        return 2 * sum;
    }
    static BigReal ILogarithmicFunctions<BigReal>.Log(BigReal value) {
        return Log(value);
    }
    /// <summary>
    /// Returns the base <paramref name="baseValue"/> logarithm of <paramref name="value"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Log(BigReal value, BigReal baseValue, int decimals = DoubleReliableDecimals) {
        if (TryCalculateAsDouble(value, baseValue, double.Log, decimals, out double result)) {
            return result;
        }

        // https://math.stackexchange.com/a/107576
        return Log(value, decimals) / Log(baseValue, decimals);
    }
    static BigReal ILogarithmicFunctions<BigReal>.Log(BigReal value, BigReal baseValue) {
        return Log(value, baseValue);
    }
    /// <summary>
    /// Returns the base 10 logarithm of <paramref name="value"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Log10(BigReal value, int decimals = DoubleReliableDecimals) {
        if (TryCalculateAsDouble(value, double.Log10, decimals, out double result)) {
            return result;
        }

        return Log(value, Ten, decimals);
    }
    static BigReal ILogarithmicFunctions<BigReal>.Log10(BigReal value) {
        return Log10(value);
    }
    /// <summary>
    /// Returns the base 2 logarithm of <paramref name="value"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Log2(BigReal value, int decimals = DoubleReliableDecimals) {
        if (TryCalculateAsDouble(value, double.Log2, decimals, out double result)) {
            return result;
        }

        return Log(value, 2, decimals);
    }
    static BigReal ILogarithmicFunctions<BigReal>.Log2(BigReal value) {
        return Log2(value);
    }
    /// <summary>
    /// Returns <paramref name="value"/> as a positive number.
    /// </summary>
    public static BigReal Abs(BigReal value) {
        return new BigReal(BigInteger.Abs(value.Numerator), BigInteger.Abs(value.Denominator));
    }
    /// <summary>
    /// Returns <paramref name="value"/> multiplied by -1.
    /// </summary>
    public static BigReal Negate(BigReal value) {
        return new BigReal(BigInteger.Negate(value.Numerator), value.Denominator);
    }
    /// <summary>
    /// Returns the reciprocal of <paramref name="value"/> (flips the fraction).
    /// </summary>
    public static BigReal Inverse(BigReal value) {
        return new BigReal(value.Denominator, value.Numerator);
    }
    /// <summary>
    /// Returns the result of adding <paramref name="value"/> and 1.
    /// </summary>
    public static BigReal Increment(BigReal value) {
        if (!IsFinite(value)) {
            return value;
        }
        return new BigReal(value.Numerator + value.Denominator, value.Denominator);
    }
    /// <summary>
    /// Returns the result of subtracting <paramref name="value"/> and 1.
    /// </summary>
    public static BigReal Decrement(BigReal value) {
        if (!IsFinite(value)) {
            return value;
        }
        return new BigReal(value.Numerator - value.Denominator, value.Denominator);
    }
    /// <summary>
    /// Returns the smallest whole value greater than or equal to <paramref name="value"/>.
    /// </summary>
    public static BigReal Ceiling(BigReal value) {
        if (!IsFinite(value)) {
            return value;
        }
        BigInteger numerator = value.Numerator;
        if (BigInteger.IsNegative(numerator)) {
            numerator -= BigInteger.Remainder(numerator, value.Denominator);
        }
        else {
            numerator += value.Denominator - BigInteger.Remainder(numerator, value.Denominator);
        }
        return new BigReal(numerator, value.Denominator);
    }
    /// <summary>
    /// Returns the largest whole value less than or equal to <paramref name="value"/>.
    /// </summary>
    public static BigReal Floor(BigReal value) {
        if (!IsFinite(value)) {
            return value;
        }
        BigInteger numerator = value.Numerator;
        if (BigInteger.IsNegative(numerator)) {
            numerator += value.Denominator - BigInteger.Remainder(numerator, value.Denominator);
        }
        else {
            numerator -= BigInteger.Remainder(numerator, value.Denominator);
        }
        return new BigReal(numerator, value.Denominator);
    }
    /// <summary>
    /// Returns the closest whole value to <paramref name="value"/>, rounding midpoints away from zero.
    /// </summary>
    /// <remarks>
    /// Note: Pass <see cref="MidpointRounding.ToEven"/> to use banker's rounding (the default for <see cref="Math.Round(double)"/>).
    /// </remarks>
    public static BigReal Round(BigReal value) {
        return Round(value, MidpointRounding.AwayFromZero);
    }
    /// <summary>
    /// Returns the closest value with <paramref name="decimals"/> decimal places to <paramref name="value"/>, rounding midpoints away from zero.
    /// </summary>
    /// <remarks>
    /// Note: Pass <see cref="MidpointRounding.ToEven"/> to use banker's rounding (the default for <see cref="Math.Round(double)"/>).
    /// </remarks>
    public static BigReal Round(BigReal value, int decimals) {
        BigReal exponent = Pow(10, decimals);
        return Round(value * exponent, MidpointRounding.AwayFromZero) / exponent;
    }
    /// <summary>
    /// Returns the closest value with <paramref name="decimals"/> decimal places to <paramref name="value"/> according to the rounding <paramref name="mode"/>.
    /// </summary>
    public static BigReal Round(BigReal value, int decimals, MidpointRounding mode) {
        BigReal exponent = Pow(10, decimals);
        return Round(value * exponent, mode) / exponent;
    }
    /// <summary>
    /// Returns the closest whole value to <paramref name="value"/> according to the rounding <paramref name="mode"/>.
    /// </summary>
    public static BigReal Round(BigReal value, MidpointRounding mode) {
        if (!IsFinite(value)) {
            return value;
        }
        switch (mode) {
            case MidpointRounding.ToEven: {
                if (Abs(GetFractionalPart(value)) == OneHalf) {
                    // Round away from zero
                    value += OneHalf * value.Sign;
                    // Move closer to zero if not even
                    if (IsOddInteger(value)) {
                        value -= One * value.Sign;
                    }
                    return value;
                }
                else {
                    goto case MidpointRounding.AwayFromZero;
                }
            }
            case MidpointRounding.AwayFromZero: {
                if (GetFractionalPart(value) >= OneHalf) {
                    return Ceiling(value);
                }
                else {
                    return Floor(value);
                }
            }
            case MidpointRounding.ToZero: {
                if (IsNegative(value)) {
                    return Ceiling(value);
                }
                else {
                    return Floor(value);
                }
            }
            case MidpointRounding.ToNegativeInfinity: {
                return Floor(value);
            }
            case MidpointRounding.ToPositiveInfinity: {
                return Ceiling(value);
            }
            default: {
                throw new NotImplementedException(mode.ToString());
            }
        }
    }
    /// <summary>
    /// Returns <paramref name="value"/> without the fractional part.
    /// </summary>
    public static BigReal Truncate(BigReal value) {
        if (!IsFinite(value)) {
            return value;
        }
        return GetWholePart(value);
    }
    /// <summary>
    /// Returns <paramref name="value"/> with the digits after <paramref name="decimals"/> decimal places zeroed.
    /// </summary>
    public static BigReal Truncate(BigReal value, int decimals) {
        BigReal exponent = Pow(10, decimals);
        return Truncate(value * exponent) / exponent;
    }
    /// <summary>
    /// Returns the part of <paramref name="value"/> before the decimal point (e.g. -12.34 -> -12).
    /// </summary>
    public static BigInteger GetWholePart(BigReal value) {
        return value.Numerator / value.Denominator;
    }
    /// <summary>
    /// Returns the part of <paramref name="value"/> after the decimal point (e.g. -12.34 -> 0.34).
    /// </summary>
    public static BigReal GetFractionalPart(BigReal value) {
        if (!IsFinite(value)) {
            return value;
        }
        return Abs(new BigReal(BigInteger.Remainder(value.Numerator, value.Denominator), value.Denominator));
    }
    /// <summary>
    /// Shifts <paramref name="value"/>'s digits to the left <paramref name="shift"/> times according to the given <paramref name="numberBase"/>.<br/>
    /// This is the same as dividing by <paramref name="numberBase"/> to the power of <paramref name="shift"/>.
    /// </summary>
    public static BigReal LeftShift(BigReal value, int shift = 1, int numberBase = 2) {
        if (!IsFinite(value)) {
            return value;
        }
        if (shift < 0) {
            return RightShift(value, -shift);
        }
        return new BigReal(value.Numerator * BigInteger.Pow(numberBase, shift), value.Denominator);
    }
    /// <summary>
    /// Shifts <paramref name="value"/>'s digits to the right <paramref name="shift"/> times according to the given <paramref name="numberBase"/>.<br/>
    /// This is the same as multiplying by <paramref name="numberBase"/> to the power of <paramref name="shift"/>.
    /// </summary>
    public static BigReal RightShift(BigReal value, int shift = 1, int numberBase = 2) {
        if (!IsFinite(value)) {
            return value;
        }
        if (shift < 0) {
            return LeftShift(value, -shift);
        }
        return new BigReal(value.Numerator, value.Denominator * BigInteger.Pow(numberBase, shift));
    }
    /// <summary>
    /// Returns the square root of <paramref name="value"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Sqrt(BigReal value, int decimals = DoubleReliableDecimals) {
        return RootN(value, 2, decimals);
    }
    static BigReal IRootFunctions<BigReal>.Sqrt(BigReal value) {
        return Sqrt(value);
    }
    /// <summary>
    /// Returns the cube root of <paramref name="value"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Cbrt(BigReal value, int decimals = DoubleReliableDecimals) {
        return RootN(value, 3, decimals);
    }
    static BigReal IRootFunctions<BigReal>.Cbrt(BigReal value) {
        return Cbrt(value);
    }
    /// <summary>
    /// Returns the n-th root of <paramref name="value"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal RootN(BigReal value, int root, int decimals = DoubleReliableDecimals) {
        if (IsZero(value) || IsOne(value)) {
            return value;
        }
        if (!IsFinite(value)) {
            return value;
        }
        if (TryCalculateAsDouble(value, root, double.RootN, decimals, out double result)) {
            return result;
        }

        // Can't root a negative number
        if (IsNegative(value)) {
            return NaN;
        }

        // Root is negative
        if (root < 0) {
            return One / Pow(value, -root);
        }

        /*
         * // Slow alternative:
         * // Use Newton's method to repeatedly get closer to the answer
         * 
         * // Convert decimals to epsilon (e.g. 3 -> 0.001)
         * BigReal epsilon = One / Pow(Ten, decimals);
         * 
         * BigReal guess = value / root;
         * while (true) {
         *     BigReal nextGuess = (((root - 1) * guess) + (value / Pow(guess, root - 1))) / root;
         *     if (Abs(nextGuess - guess) <= epsilon) {
         *         guess = nextGuess;
         *         break;
         *     }
         *     guess = nextGuess;
         * }
         * return guess;
         */

        // Use the identity root_n(x) = e^(ln(x)/n)
        return Exp(Log(value, decimals: decimals) / root, decimals: decimals);
    }
    static BigReal IRootFunctions<BigReal>.RootN(BigReal value, int root) {
        return RootN(value, root);
    }
    /// <summary>
    /// Returns e raised to the specified <paramref name="power"/>.
    /// </summary>
    public static BigReal Exp(BigReal power, int decimals = DoubleReliableDecimals) {
        if (IsNaN(power)) {
            return NaN;
        }
        if (IsPositiveInfinity(power)) {
            return PositiveInfinity;
        }
        if (IsNegativeInfinity(power)) {
            return Zero;
        }
        if (IsZero(power)) {
            return One;
        }
        if (IsNegative(power)) {
            return One / Exp(Abs(power));
        }
        if (IsOne(power)) {
            return E;
        }
        if (TryCalculateAsDouble(power, double.Exp, decimals, out double result)) {
            return result;
        }

        // Convert decimals to epsilon
        BigReal epsilon = One / Pow(Ten, decimals);

        BigReal term = One; // first term = 1
        BigReal sum = One; // start sum = 1
        long n = 1;

        while (true) {
            term *= power / n; // term = power^n / n!
            if (Abs(term) <= epsilon) {
                break;
            }
            sum += term;
            n++;
        }

        return Round(sum, decimals);
    }
    /// <summary>
    /// Returns the hypotenuse of <paramref name="x"/> and <paramref name="y"/>, correct to <paramref name="decimals"/> decimal places.<br/>
    /// This is the same as <c>sqrt(pow(<paramref name="x"/>, 2) + pow(<paramref name="y"/>, 2))</c>.
    /// </summary>
    public static BigReal Hypot(BigReal x, BigReal y, int decimals = DoubleReliableDecimals) {
        return Sqrt(Pow(x, 2) + Pow(y, 2), decimals);
    }
    static BigReal IRootFunctions<BigReal>.Hypot(BigReal x, BigReal y) {
        return Hypot(x, y);
    }
    /// <summary>
    /// Factorizes the numerator and denominator to their canonical form.
    /// </summary>
    /// <remarks>
    /// This can be very slow, so use only when necessary (consider leaving the fraction in an unsimplified form).
    /// </remarks>
    public static BigReal Simplify(BigReal value) {
        if (!IsFinite(value)) {
            return value;
        }
        if (value.Denominator.IsOne || value.Denominator.IsZero) {
            return value;
        }
        BigInteger factor = BigInteger.GreatestCommonDivisor(value.Numerator, value.Denominator);
        return new BigReal(value.Numerator / factor, value.Denominator / factor);
    }
    /// <summary>
    /// Ensures the denominator of <paramref name="value"/> is positive.
    /// </summary>
    public static BigReal SimplifySigns(BigReal value) {
        if (BigInteger.IsNegative(value.Denominator)) {
            return new BigReal(-value.Numerator, -value.Denominator);
        }
        return value;
    }
    /// <summary>
    /// Parses a value from <paramref name="input"/>.
    /// </summary>
    public static BigReal Parse(string input) {
        return Parse(input, null);
    }
    /// <summary>
    /// Parses a value from <paramref name="input"/>.
    /// </summary>
    public static BigReal Parse(string input, IFormatProvider? provider) {
        return Parse(input, NumberStyles.Any, provider);
    }
    /// <summary>
    /// Parses a value from <paramref name="input"/>.
    /// </summary>
    public static BigReal Parse(string input, NumberStyles style, IFormatProvider? provider) {
        NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

        // Try parse named literal
        ReadOnlySpan<char> trimmedValue = input.AsSpan().Trim();
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

        // Find exponent
        int exponentPos = input.IndexOf('e', StringComparison.OrdinalIgnoreCase);
        // Found exponent
        BigReal exponent = Zero;
        if (exponentPos > 0) {
            // Get exponent
            exponent = Parse(input[(exponentPos + 1)..], style, provider);
            // Remove exponent
            input = input[..exponentPos];
        }

        // Find decimal point
        int decimalPointPos = input.IndexOf(numberFormat.NumberDecimalSeparator, StringComparison.OrdinalIgnoreCase);

        BigReal result;
        // No decimal point
        if (decimalPointPos < 0) {
            // Parse integer
            result = BigInteger.Parse(input, style, provider);
        }
        else {
            // Remove decimal point
            input = input.Replace(numberFormat.NumberDecimalSeparator, "", StringComparison.OrdinalIgnoreCase);
            // Get numerator and denominator
            BigInteger numerator = BigInteger.Parse(input, style, provider);
            BigInteger denominator = BigInteger.Pow(10, input.Length - decimalPointPos);
            result = new BigReal(numerator, denominator);
        }

        // Multiply by 10 ^ exponent
        if (!IsZero(exponent)) {
            result *= Pow(Ten, exponent);
        }
        return result;
    }
    /// <summary>
    /// Parses a value from <paramref name="input"/>.
    /// </summary>
    public static BigReal Parse(scoped ReadOnlySpan<char> input) {
        return Parse(input, null);
    }
    /// <summary>
    /// Parses a value from <paramref name="input"/>.
    /// </summary>
    public static BigReal Parse(scoped ReadOnlySpan<char> input, IFormatProvider? provider) {
        return Parse(input, NumberStyles.Any, provider);
    }
    /// <summary>
    /// Parses a value from <paramref name="input"/>.
    /// </summary>
    public static BigReal Parse(scoped ReadOnlySpan<char> input, NumberStyles style, IFormatProvider? provider) {
        return Parse(input.ToString(), style, provider);
    }
    /// <summary>
    /// Tries to parse a value from <paramref name="input"/>.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? input, out BigReal result) {
        return TryParse(input, null, out result);
    }
    /// <summary>
    /// Tries to parse a value from <paramref name="input"/>.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, out BigReal result) {
        return TryParse(input, NumberStyles.Any, provider, out result);
    }
    /// <summary>
    /// Tries to parse a value from <paramref name="input"/>.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? input, NumberStyles style, IFormatProvider? provider, out BigReal result) {
        if (input is null) {
            result = default;
            return false;
        }
        try {
            result = Parse(input, style, provider);
            return true;
        }
        catch (Exception) {
            result = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to parse a value from <paramref name="input"/>.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> input, out BigReal result) {
        return TryParse(input, null, out result);
    }
    /// <summary>
    /// Tries to parse a value from <paramref name="input"/>.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? provider, out BigReal result) {
        return TryParse(input, NumberStyles.Any, provider, out result);
    }
    /// <summary>
    /// Tries to parse a value from <paramref name="input"/>.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> input, NumberStyles style, IFormatProvider? provider, out BigReal result) {
        return TryParse(input.ToString(), style, provider, out result);
    }
    /// <summary>
    /// Creates a <see cref="BigReal"/> from the given <typeparamref name="T"/>.
    /// </summary>
    public static BigReal Create<T>(T value) where T : INumberBase<T> {
        return value switch {
            sbyte sbyteValue => sbyteValue,
            byte byteValue => byteValue,
            short shortValue => shortValue,
            ushort ushortValue => ushortValue,
            int intValue => intValue,
            uint uintValue => uintValue,
            long longValue => longValue,
            ulong ulongValue => ulongValue,
            Int128 int128Value => int128Value,
            UInt128 uint128Value => uint128Value,
            char charValue => charValue,
            nint nintValue => nintValue,
            nuint nuintValue => nuintValue,
            BigInteger bigIntegerValue => bigIntegerValue,
            Half halfValue => halfValue,
            float floatValue => floatValue,
            double doubleValue => doubleValue,
            decimal decimalValue => decimalValue,
            BigReal bigRealValue => bigRealValue,
            _ => Parse(value.ToString(null, null)),
        };
    }
    /// <summary>
    /// Tries to create a <see cref="BigReal"/> from the given <typeparamref name="T"/>.
    /// </summary>
    public static bool TryCreate<T>(T? value, out BigReal result) where T : INumberBase<T> {
        if (value is null) {
            result = default;
            return false;
        }
        try {
            result = Create(value);
            return true;
        }
        catch (Exception) {
            result = default;
            return false;
        }
    }
    /// <summary>
    /// Returns:
    /// <list type="bullet">
    ///   <item>1 if <paramref name="left"/> &gt; <paramref name="right"/></item>
    ///   <item>0 if <paramref name="left"/> == <paramref name="right"/></item>
    ///   <item>-1 if <paramref name="left"/> &lt; <paramref name="right"/></item>
    /// </list>
    /// </summary>
    public static int Compare(BigReal left, BigReal right) {
        return left.CompareTo(right);
    }
    /// <summary>
    /// Returns whether <paramref name="left"/> is exactly equal to <paramref name="right"/>.
    /// </summary>
    /// <remarks>
    /// This method returns <see langword="true"/> for <c>Equals(NaN, NaN)</c>.
    /// </remarks>
    public static bool Equals(BigReal left, BigReal right) {
        return left.Equals(right);
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is in its simplest form.
    /// </summary>
    public static bool IsCanonical(BigReal value) {
        return BigInteger.GreatestCommonDivisor(value.Numerator, value.Denominator).IsOne;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is a whole number.
    /// </summary>
    public static bool IsInteger(BigReal value) {
        return BigInteger.Remainder(value.Numerator, value.Denominator).IsZero;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is a whole number divisible by 2.
    /// </summary>
    public static bool IsEvenInteger(BigReal value) {
        return IsInteger(value) && GetWholePart(value).IsEven;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is a whole number not divisible by 2.
    /// </summary>
    public static bool IsOddInteger(BigReal value) {
        return IsInteger(value) && !GetWholePart(value).IsEven;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is not a number.
    /// </summary>
    public static bool IsNaN(BigReal value) {
        return value.Numerator.IsZero && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is not infinity or NaN.
    /// </summary>
    public static bool IsFinite(BigReal value) {
        return !value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> represents positive or negative infinity.
    /// </summary>
    public static bool IsInfinity(BigReal value) {
        return !value.Numerator.IsZero && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> represents positive infinity.
    /// </summary>
    public static bool IsPositiveInfinity(BigReal value) {
        return value.Numerator.Sign > 0 && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> represents negative infinity.
    /// </summary>
    public static bool IsNegativeInfinity(BigReal value) {
        return value.Numerator.Sign < 0 && value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is greater than or equal to zero.
    /// </summary>
    public static bool IsPositive(BigReal value) {
        return BigInteger.IsPositive(value.Numerator) ^ BigInteger.IsPositive(value.Denominator);
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is less than zero.
    /// </summary>
    public static bool IsNegative(BigReal value) {
        return BigInteger.IsNegative(value.Numerator) ^ BigInteger.IsNegative(value.Denominator);
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is zero.
    /// </summary>
    public static bool IsZero(BigReal value) {
        return value.Numerator.IsZero && !value.Denominator.IsZero;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is one.
    /// </summary>
    public static bool IsOne(BigReal value) {
        return !value.Numerator.IsZero && !value.Denominator.IsZero && value.Numerator == value.Denominator;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is one.
    /// </summary>
    public static bool IsNegativeOne(BigReal value) {
        return IsNegative(value) ? IsOne(Negate(value)) : IsOne(value);
    }
    /// <summary>
    /// Returns the greater of <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    public static BigReal Max(BigReal a, BigReal b) {
        if (IsNaN(a) || IsNaN(b)) {
            return NaN;
        }
        return a > b ? a : b;
    }
    /// <summary>
    /// Returns the lesser of <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    public static BigReal Min(BigReal a, BigReal b) {
        if (IsNaN(a) || IsNaN(b)) {
            return NaN;
        }
        return a < b ? a : b;
    }
    /// <summary>
    /// Returns the linear interpolation between <paramref name="from"/> and <paramref name="to"/> by <paramref name="weight"/>.
    /// </summary>
    public static BigReal Lerp(BigReal from, BigReal to, BigReal weight) {
        if (IsNaN(from) || IsNaN(to) || IsNaN(weight)) {
            return NaN;
        }
        return from + ((to - from) * weight);
    }
    /// <summary>
    /// Returns the inverse linear interpolation between <paramref name="from"/> and <paramref name="to"/> by <paramref name="weight"/>.
    /// </summary>
    public static BigReal InverseLerp(BigReal from, BigReal to, BigReal weight) {
        if (IsNaN(from) || IsNaN(to) || IsNaN(weight)) {
            return NaN;
        }
        return (weight - from) / (to - from);
    }
    /// <summary>
    /// Returns whether <paramref name="input"/> is within the supported range of <typeparamref name="T"/>.
    /// </summary>
    private static bool IsInRangeOf<T>(BigReal input) where T : INumberBase<T>, IMinMaxValue<T> {
        if (input > Create(T.MaxValue)) {
            return false;
        }
        if (input < Create(T.MinValue)) {
            return false;
        }
        return true;
    }

    #endregion

    #region Instance Methods

    /// <summary>
    /// Stringifies this value as a decimal, truncating at 15 decimal places.
    /// </summary>
    public override string ToString() {
        return ToString(null);
    }
    /// <summary>
    /// Stringifies this value as a decimal, truncating at 15 decimal places.
    /// </summary>
    public string ToString(IFormatProvider? provider) {
        return ToString(decimals: 15, provider);
    }
    /// <summary>
    /// Stringifies this value as a decimal, truncating at <paramref name="decimals"/> decimal places.<br/>
    /// The number is optionally padded with a decimal (<c>.0</c>).
    /// </summary>
    public string ToString(int decimals, IFormatProvider? provider = null, bool padDecimal = false) {
        NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

        string StringifyInteger(string wholeString) {
            if (padDecimal) {
                return wholeString + numberFormat.NumberDecimalSeparator + "0";
            }
            return wholeString;
        }

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

        // Ensure whole number has negative sign if zero and negative (e.g. -0.5)
        if (!BigInteger.IsNegative(whole) && IsNegative(this)) {
            wholeString = numberFormat.NegativeSign + wholeString;
        }

        // Number is whole
        if (remainder.IsZero) {
            return StringifyInteger(wholeString);
        }

        // Get number as positive scaled integer (e.g. 123.45 -> 1234500000)
        BigInteger fractional = BigInteger.Abs(Numerator * BigInteger.Pow(10, decimals) / Denominator);

        // Get fraction part (e.g. 123.45 -> 4500000)
        using ValueStringBuilder fractionBuilder = new(stackalloc char[int.Min(decimals, 128)]);
        for (int columnNumber = 0; columnNumber < decimals; columnNumber++) {
            // Append each digit of fraction
            fractionBuilder.Append(fractional % 10, bufferSize: 1);
            fractional /= 10;
        }
        fractionBuilder.Reverse();
        fractionBuilder.TrimEnd('0');
        ReadOnlySpan<char> fractionSpan = fractionBuilder.AsSpan();

        // Number at given precision is whole
        if (fractionSpan.IsEmpty) {
            return StringifyInteger(wholeString);
        }

        // Combine parts
        return string.Concat(wholeString, numberFormat.NumberDecimalSeparator, fractionSpan);
    }
    /// <summary>
    /// Stringifies this value as a simplified rational (numerator / denominator).
    /// </summary>
    public string ToRationalString() {
        BigReal value = Simplify(this);
        return value.Numerator + " / " + value.Denominator;
    }
    /// <summary>
    /// Returns:
    /// <list type="bullet">
    ///   <item>1 if this value &gt; <paramref name="other"/></item>
    ///   <item>0 if this value == <paramref name="other"/></item>
    ///   <item>-1 if this value &lt; <paramref name="other"/></item>
    /// </list>
    /// </summary>
    public int CompareTo(BigReal other) {
        if (IsNaN(this) || IsNaN(other)) {
            if (IsNaN(this) && !IsNaN(other)) {
                return -1;
            }
            else if (!IsNaN(this) && IsNaN(other)) {
                return 1;
            }
            else {
                return 0;
            }
        }
        BigReal left = SimplifySigns(this);
        BigReal right = SimplifySigns(other);
        return BigInteger.Compare(left.Numerator * right.Denominator, right.Numerator * left.Denominator);
    }
    /// <summary>
    /// Returns:
    /// <list type="bullet">
    ///   <item>1 if this value &gt; <paramref name="other"/></item>
    ///   <item>0 if this value == <paramref name="other"/></item>
    ///   <item>-1 if this value &lt; <paramref name="other"/></item>
    ///   <item>1 if <paramref name="other"/> is <see langword="null"/> (<see langword="null"/> is less than any value)</item>
    ///   <item>throws an exception if <paramref name="other"/> is not <see cref="BigReal"/></item>
    /// </list>
    /// </summary>
    public int CompareTo(object? other) {
        if (other is null) {
            return 1;
        }
        else if (other is BigReal otherBigReal) {
            return CompareTo(otherBigReal);
        }
        else {
            throw new ArgumentException($"{nameof(other)} is not {nameof(BigReal)}");
        }
    }
    /// <summary>
    /// Returns whether this value is exactly equal to <paramref name="other"/>.
    /// </summary>
    /// <remarks>
    /// This method returns <see langword="true"/> for <c>NaN.Equals(NaN)</c>.
    /// </remarks>
    public bool Equals(BigReal other) {
        if (!IsFinite(this)) {
            if (IsNaN(this)) {
                return IsNaN(other);
            }
            if (IsPositiveInfinity(this)) {
                return IsPositiveInfinity(other);
            }
            if (IsNegativeInfinity(this)) {
                return IsNegativeInfinity(other);
            }
        }
        if (!IsFinite(other)) {
            if (IsNaN(other)) {
                return IsNaN(this);
            }
            if (IsPositiveInfinity(other)) {
                return IsPositiveInfinity(this);
            }
            if (IsNegativeInfinity(other)) {
                return IsNegativeInfinity(this);
            }
        }
        return other.Numerator * Denominator == Numerator * other.Denominator;
    }
    /// <summary>
    /// Returns whether <paramref name="other"/> is a value and this value is exactly equal to <paramref name="other"/>.
    /// </summary>
    public override bool Equals(object? other) {
        return other is BigReal otherBigReal && Equals(otherBigReal);
    }
    /// <summary>
    /// Returns a dictionary hash code for the numerator and denominator of this value.
    /// </summary>
    public override int GetHashCode() {
        return HashCode.Combine(Numerator, Denominator);
    }
    /// <summary>
    /// Returns the numerator and denominator of this value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deconstruct(out BigInteger numerator, out BigInteger denominator) {
        (numerator, denominator) = (Numerator, Denominator);
    }

    #endregion

    #region Operators

    /// <inheritdoc cref="Negate(BigReal)"/>
    public static BigReal operator -(BigReal value) {
        return Negate(value);
    }
    /// <inheritdoc cref="Subtract(BigReal, BigReal)"/>
    public static BigReal operator -(BigReal value, BigReal other) {
        return Subtract(value, other);
    }
    /// <inheritdoc cref="Decrement(BigReal)"/>
    public static BigReal operator --(BigReal value) {
        return Decrement(value);
    }
    /// <summary>
    /// Returns <paramref name="value"/> verbatim.
    /// </summary>
    public static BigReal operator +(BigReal value) {
        return value;
    }
    /// <inheritdoc cref="Add(BigReal, BigReal)"/>
    public static BigReal operator +(BigReal value, BigReal other) {
        return Add(value, other);
    }
    /// <inheritdoc cref="Increment(BigReal)"/>
    public static BigReal operator ++(BigReal value) {
        return Increment(value);
    }
    /// <inheritdoc cref="Remainder(BigReal, BigReal)"/>
    public static BigReal operator %(BigReal value, BigReal other) {
        return Remainder(value, other);
    }
    /// <inheritdoc cref="Multiply(BigReal, BigReal)"/>
    public static BigReal operator *(BigReal value, BigReal other) {
        return Multiply(value, other);
    }
    /// <inheritdoc cref="Divide(BigReal, BigReal)"/>
    public static BigReal operator /(BigReal value, BigReal other) {
        return Divide(value, other);
    }
    /// <inheritdoc cref="LeftShift(BigReal, int, int)"/>
    public static BigReal operator <<(BigReal value, int shift) {
        return LeftShift(value, shift);
    }
    /// <inheritdoc cref="RightShift(BigReal, int, int)"/>
    public static BigReal operator >>(BigReal value, int shift) {
        return RightShift(value, shift);
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is exactly equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator ==(BigReal value, BigReal other) {
        if (IsNaN(value) || IsNaN(other)) {
            return false;
        }
        return Equals(value, other);
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is not exactly equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator !=(BigReal value, BigReal other) {
        if (IsNaN(value) || IsNaN(other)) {
            return false;
        }
        return !Equals(value, other);
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is less than <paramref name="other"/>.
    /// </summary>
    public static bool operator <(BigReal value, BigReal other) {
        if (IsNaN(value) || IsNaN(other)) {
            return false;
        }
        return Compare(value, other) < 0;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is less than or equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator <=(BigReal value, BigReal other) {
        if (IsNaN(value) || IsNaN(other)) {
            return false;
        }
        return Compare(value, other) <= 0;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is greater than <paramref name="other"/>.
    /// </summary>
    public static bool operator >(BigReal value, BigReal other) {
        if (IsNaN(value) || IsNaN(other)) {
            return false;
        }
        return Compare(value, other) > 0;
    }
    /// <summary>
    /// Returns whether <paramref name="value"/> is greater than or equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator >=(BigReal value, BigReal other) {
        if (IsNaN(value) || IsNaN(other)) {
            return false;
        }
        return Compare(value, other) >= 0;
    }

    #endregion

    #region Casts From BigReal

    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="sbyte"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator sbyte(BigReal value) => NarrowConvert(value, sbyte.MinValue, sbyte.MaxValue, value => (sbyte)((sbyte)value.Numerator / (sbyte)value.Denominator));
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="byte"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator byte(BigReal value) => NarrowConvert(value, byte.MinValue, byte.MaxValue, value => (byte)((byte)value.Numerator / (byte)value.Denominator));
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="short"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator short(BigReal value) => NarrowConvert(value, short.MinValue, short.MaxValue, value => (short)((short)value.Numerator / (short)value.Denominator));
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="ushort"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator ushort(BigReal value) => NarrowConvert(value, ushort.MinValue, ushort.MaxValue, value => (ushort)((ushort)value.Numerator / (ushort)value.Denominator));
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="int"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator int(BigReal value) => NarrowConvert(value, int.MinValue, int.MaxValue, value => (int)value.Numerator / (int)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="uint"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator uint(BigReal value) => NarrowConvert(value, uint.MinValue, uint.MaxValue, value => (uint)value.Numerator / (uint)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="long"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator long(BigReal value) => NarrowConvert(value, long.MinValue, long.MaxValue, value => (long)value.Numerator / (long)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="ulong"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator ulong(BigReal value) => NarrowConvert(value, ulong.MinValue, ulong.MaxValue, value => (ulong)value.Numerator / (ulong)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="Int128"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator Int128(BigReal value) => NarrowConvert(value, Int128.MinValue, Int128.MaxValue, value => (Int128)value.Numerator / (Int128)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="UInt128"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator UInt128(BigReal value) => NarrowConvert(value, UInt128.MinValue, UInt128.MaxValue, value => (UInt128)value.Numerator / (UInt128)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="char"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator char(BigReal value) => NarrowConvert(value, char.MinValue, char.MaxValue, value => (char)((char)value.Numerator / (char)value.Denominator));
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="Rune"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator Rune(BigReal value) => new((uint)value);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="nint"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator nint(BigReal value) => NarrowConvert(value, nint.MinValue, nint.MaxValue, value => (nint)value.Numerator / (nint)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="nuint"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator nuint(BigReal value) => NarrowConvert(value, nuint.MinValue, nuint.MaxValue, value => (nuint)value.Numerator / (nuint)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="BigInteger"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator BigInteger(BigReal value) => GetWholePart(value);

    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="Half"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator Half(BigReal value) => NarrowConvert(value, Half.MinValue, Half.MaxValue, value => (Half)value.Numerator / (Half)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="float"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator float(BigReal value) => NarrowConvert(value, float.MinValue, float.MaxValue, value => (float)value.Numerator / (float)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="double"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator double(BigReal value) => NarrowConvert(value, double.MinValue, double.MaxValue, value => (double)value.Numerator / (double)value.Denominator);
    /// <summary>
    /// Converts from <see cref="BigReal"/> to <see cref="decimal"/> using a narrowing conversion.
    /// </summary>
    public static explicit operator decimal(BigReal value) => NarrowConvert(value, decimal.MinValue, decimal.MaxValue, value => (decimal)value.Numerator / (decimal)value.Denominator);

    /// <summary>
    /// Attempts to convert <paramref name="value"/> to <typeparamref name="T"/>, throwing if the value is outside the supported range.
    /// </summary>
    private static T NarrowConvert<T>(BigReal value, BigReal minValue, BigReal maxValue, Func<BigReal, T> convert) {
        if (value < minValue) {
            throw new OverflowException($"{nameof(value)} is less than {typeof(T).Name}.MinValue.");
        }
        if (value > maxValue) {
            throw new OverflowException($"{nameof(value)} is greater than {typeof(T).Name}.MaxValue.");
        }
        return convert(value);
    }

    #endregion

    #region Casts To BigReal

    /// <summary>
    /// Converts from <see cref="sbyte"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(sbyte value) => new(value);
    /// <summary>
    /// Converts from <see cref="byte"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(byte value) => new(value);
    /// <summary>
    /// Converts from <see cref="short"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(short value) => new(value);
    /// <summary>
    /// Converts from <see cref="ushort"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(ushort value) => new(value);
    /// <summary>
    /// Converts from <see cref="int"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(int value) => new(value);
    /// <summary>
    /// Converts from <see cref="uint"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(uint value) => new(value);
    /// <summary>
    /// Converts from <see cref="long"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(long value) => new(value);
    /// <summary>
    /// Converts from <see cref="ulong"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(ulong value) => new(value);
    /// <summary>
    /// Converts from <see cref="Int128"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(Int128 value) => new(value);
    /// <summary>
    /// Converts from <see cref="UInt128"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(UInt128 value) => new(value);
    /// <summary>
    /// Converts from <see cref="char"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(char value) => new(value);
    /// <summary>
    /// Converts from <see cref="Rune"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(Rune value) => new(value);
    /// <summary>
    /// Converts from <see cref="nint"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(nint value) => new(value);
    /// <summary>
    /// Converts from <see cref="nuint"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(nuint value) => new(value);
    /// <summary>
    /// Converts from <see cref="BigInteger"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(BigInteger value) => new(value);

    /// <summary>
    /// Converts from <see cref="Half"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(Half value) => new(value);
    /// <summary>
    /// Converts from <see cref="float"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(float value) => new(value);
    /// <summary>
    /// Converts from <see cref="double"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(double value) => new(value);
    /// <summary>
    /// Converts from <see cref="decimal"/> to <see cref="BigReal"/>.
    /// </summary>
    public static implicit operator BigReal(decimal value) => new(value);

    #endregion

    #region Float Helpers

    // Original double-to-fraction helper from:
    // https://ericlippert.com/2015/11/30/the-dedoublifier-part-one
    // https://ericlippert.com/2015/12/03/the-dedoublifier-part-two

    private readonly struct HalfHelper(Half value) {
        public Half Value { get; } = value;
        public ushort RawBits { get; } = BitConverter.HalfToUInt16Bits(value);
        public int RawSign => (RawBits >> 15) & 0x1;
        public long RawMantissa => RawBits & 0x03FF;
        public int RawExponent => (RawBits >> 10) & 0x1F;
        public bool IsNaN => RawExponent == 0x1F && RawMantissa != 0;
        public bool IsInfinity => RawExponent == 0x1F && RawMantissa == 0;
        public bool IsZero => RawExponent == 0 && RawMantissa == 0;
        public bool IsDenormal => RawExponent == 0 && RawMantissa != 0;

        public bool IsNegative => RawSign != 0;
        public int Sign => IsZero ? 0 : (IsNegative ? -1 : 1);
        public bool IsPositiveInfinity => !IsNegative && IsInfinity;
        public bool IsNegativeInfinity => IsNegative && IsInfinity;

        public long Mantissa {
            get {
                if (IsZero || IsDenormal || IsNaN || IsInfinity) {
                    return RawMantissa;
                }
                else {
                    return RawMantissa | 0x0400;
                }
            }
        }
        public int Exponent {
            get {
                if (IsZero) {
                    return 0;
                }
                else if (IsDenormal) {
                    return -24;
                }
                else if (IsNaN || IsInfinity) {
                    return RawExponent;
                }
                else {
                    return RawExponent - 25;
                }
            }
        }

        public BigReal ToBigReal() {
            if (IsPositiveInfinity) {
                return PositiveInfinity;
            }
            if (IsNegativeInfinity) {
                return NegativeInfinity;
            }
            if (IsNaN) {
                return NaN;
            }
            if (IsZero) {
                return Zero;
            }

            BigInteger numerator;
            BigInteger denominator;
            if (Exponent >= 0) {
                numerator = Sign * Mantissa * BigInteger.Pow(2, Exponent);
                denominator = BigInteger.One;
            }
            else {
                numerator = Sign * Mantissa;
                denominator = BigInteger.Pow(2, -Exponent);
            }
            return new BigReal(numerator, denominator);
        }
    }
    private readonly struct SingleHelper(float value) {
        public float Value { get; } = value;
        public uint RawBits { get; } = BitConverter.SingleToUInt32Bits(value);
        public int RawSign => (int)(RawBits >> 31);
        public long RawMantissa => RawBits & 0x007FFFFF;
        public int RawExponent => (int)(RawBits >> 23) & 0xFF;
        public bool IsNaN => RawExponent == 0xFF && RawMantissa != 0;
        public bool IsInfinity => RawExponent == 0xFF && RawMantissa == 0;
        public bool IsZero => RawExponent == 0 && RawMantissa == 0;
        public bool IsDenormal => RawExponent == 0 && RawMantissa != 0;

        public bool IsNegative => RawSign != 0;
        public int Sign => IsZero ? 0 : (IsNegative ? -1 : 1);
        public bool IsPositiveInfinity => !IsNegative && IsInfinity;
        public bool IsNegativeInfinity => IsNegative && IsInfinity;

        public long Mantissa {
            get {
                if (IsZero || IsDenormal || IsNaN || IsInfinity) {
                    return RawMantissa;
                }
                else {
                    return RawMantissa | 0x00800000;
                }
            }
        }
        public int Exponent {
            get {
                if (IsZero) {
                    return 0;
                }
                else if (IsDenormal) {
                    return -149;
                }
                else if (IsNaN || IsInfinity) {
                    return RawExponent;
                }
                else {
                    return RawExponent - 150;
                }
            }
        }

        public BigReal ToBigReal() {
            if (IsPositiveInfinity) {
                return PositiveInfinity;
            }
            if (IsNegativeInfinity) {
                return NegativeInfinity;
            }
            if (IsNaN) {
                return NaN;
            }
            if (IsZero) {
                return Zero;
            }

            BigInteger numerator;
            BigInteger denominator;
            if (Exponent >= 0) {
                numerator = Sign * Mantissa * BigInteger.Pow(2, Exponent);
                denominator = BigInteger.One;
            }
            else {
                numerator = Sign * Mantissa;
                denominator = BigInteger.Pow(2, -Exponent);
            }
            return new BigReal(numerator, denominator);
        }
    }
    private readonly struct DoubleHelper(double value) {
        public double Value { get; } = value;
        public ulong RawBits { get; } = BitConverter.DoubleToUInt64Bits(value);
        public int RawSign => (int)(RawBits >> 63);
        public long RawMantissa => (long)(RawBits & 0x000FFFFFFFFFFFFF);
        public int RawExponent => (int)(RawBits >> 52) & 0x7FF;
        public bool IsNaN => RawExponent == 0x7FF && RawMantissa != 0;
        public bool IsInfinity => RawExponent == 0x7FF && RawMantissa == 0;
        public bool IsZero => RawExponent == 0 && RawMantissa == 0;
        public bool IsDenormal => RawExponent == 0 && RawMantissa != 0;

        public bool IsNegative => RawSign != 0;
        public int Sign => IsZero ? 0 : (IsNegative ? -1 : 1);
        public bool IsPositiveInfinity => !IsNegative && IsInfinity;
        public bool IsNegativeInfinity => IsNegative && IsInfinity;

        public long Mantissa {
            get {
                if (IsZero || IsDenormal || IsNaN || IsInfinity) {
                    return RawMantissa;
                }
                else {
                    return RawMantissa | 0x0010000000000000;
                }
            }
        }
        public int Exponent {
            get {
                if (IsZero) {
                    return 0;
                }
                else if (IsDenormal) {
                    return -1074;
                }
                else if (IsNaN || IsInfinity) {
                    return RawExponent;
                }
                else {
                    return RawExponent - 1075;
                }
            }
        }

        public BigReal ToBigReal() {
            if (IsPositiveInfinity) {
                return PositiveInfinity;
            }
            if (IsNegativeInfinity) {
                return NegativeInfinity;
            }
            if (IsNaN) {
                return NaN;
            }
            if (IsZero) {
                return Zero;
            }

            BigInteger numerator;
            BigInteger denominator;
            if (Exponent >= 0) {
                numerator = Sign * Mantissa * BigInteger.Pow(2, Exponent);
                denominator = BigInteger.One;
            }
            else {
                numerator = Sign * Mantissa;
                denominator = BigInteger.Pow(2, -Exponent);
            }
            return new BigReal(numerator, denominator);
        }
    }
    private readonly struct DecimalHelper(decimal value) {
        public decimal Value { get; } = value;
        public (int Low, int Mid, int High, int Flags) RawBits { get; } = GetRawBits(value);

        public bool IsNegative => (RawBits.Flags & unchecked((int)0x80000000)) != 0;
        public int RawSign => IsZero ? 0 : (IsNegative ? -1 : 1);
        public bool IsZero => RawBits.Low == 0 && RawBits.Mid == 0 && RawBits.High == 0;

        public int Sign => IsZero ? 0 : (IsNegative ? -1 : 1);

        public Int128 Mantissa {
            get {
                return (Int128)(uint)RawBits.Low
                    | ((Int128)(uint)RawBits.Mid << 32)
                    | ((Int128)(uint)RawBits.High << 64);
            }
        }
        public int Exponent => (RawBits.Flags >> 16) & 0xFF;

        public BigReal ToBigReal() {
            if (IsZero) {
                return Zero;
            }

            BigInteger numerator = Sign * Mantissa;
            BigInteger denominator = BigInteger.Pow(10, Exponent);

            return new BigReal(numerator, denominator);
        }

        private static (int Low, int Mid, int High, int Flags) GetRawBits(decimal value) {
            Span<int> bits = stackalloc int[4];
            decimal.GetBits(value, bits);
            return (bits[0], bits[1], bits[2], bits[3]);
        }
    }

    #endregion

    #region INumber

    /// <inheritdoc/>
    static int INumberBase<BigReal>.Radix => 2;

    /// <inheritdoc/>
    public static bool IsComplexNumber(BigReal value) {
        return false;
    }
    /// <inheritdoc/>
    public static bool IsImaginaryNumber(BigReal value) {
        return false;
    }
    /// <inheritdoc/>
    public static bool IsNormal(BigReal value) {
        return !IsNaN(value) && !IsInfinity(value) && !IsZero(value);
    }
    /// <inheritdoc/>
    public static bool IsSubnormal(BigReal value) {
        return false;
    }
    /// <inheritdoc/>
    public static bool IsRealNumber(BigReal value) {
        return !IsNaN(value);
    }
    /// <inheritdoc/>
    public static BigReal MaxMagnitude(BigReal x, BigReal y) {
        return Max(Abs(x), Abs(y));
    }
    /// <inheritdoc/>
    public static BigReal MaxMagnitudeNumber(BigReal x, BigReal y) {
        if (IsNaN(x)) {
            return y;
        }
        if (IsNaN(y)) {
            return x;
        }
        return MaxMagnitude(x, y);
    }
    /// <inheritdoc/>
    public static BigReal MinMagnitude(BigReal x, BigReal y) {
        return Min(Abs(x), Abs(y));
    }
    /// <inheritdoc/>
    public static BigReal MinMagnitudeNumber(BigReal x, BigReal y) {
        if (IsNaN(x)) {
            return y;
        }
        if (IsNaN(y)) {
            return x;
        }
        return MinMagnitude(x, y);
    }
    /// <inheritdoc/>
    static bool INumberBase<BigReal>.TryConvertFromChecked<TOther>(TOther value, out BigReal result) {
        return TryCreate(value, out result);
    }
    /// <inheritdoc/>
    static bool INumberBase<BigReal>.TryConvertFromSaturating<TOther>(TOther value, out BigReal result) {
        return TryCreate(value, out result);
    }
    /// <inheritdoc/>
    static bool INumberBase<BigReal>.TryConvertFromTruncating<TOther>(TOther value, out BigReal result) {
        return TryCreate(value, out result);
    }
    /// <inheritdoc/>
    static bool INumberBase<BigReal>.TryConvertToChecked<TOther>(BigReal value, [MaybeNullWhen(false)] out TOther result) {
        return TOther.TryConvertFromChecked(value, out result);
    }
    /// <inheritdoc/>
    static bool INumberBase<BigReal>.TryConvertToSaturating<TOther>(BigReal value, [MaybeNullWhen(false)] out TOther result) {
        return TOther.TryConvertFromSaturating(value, out result);
    }
    /// <inheritdoc/>
    static bool INumberBase<BigReal>.TryConvertToTruncating<TOther>(BigReal value, [MaybeNullWhen(false)] out TOther result) {
        return TOther.TryConvertFromTruncating(value, out result);
    }
    /// <inheritdoc/>
    string IFormattable.ToString(string? format, IFormatProvider? provider) {
        if (string.IsNullOrEmpty(format)) {
            return ToString(provider);
        }
        throw new NotImplementedException($"Format strings not implemented on {nameof(BigReal)}");
    }
    /// <inheritdoc/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) {
        if (format.IsEmpty) {
            string result = ToString(provider);

            if (!result.TryCopyTo(destination)) {
                charsWritten = default;
                return false;
            }

            charsWritten = result.Length;
            return true;
        }
        throw new NotImplementedException($"Format strings not implemented on {nameof(BigReal)}");
    }
    /// <inheritdoc/>
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) {
        return ((IUtf8SpanFormattable)this).TryFormat(utf8Destination, out bytesWritten, format, provider);
    }

    #endregion

    #region IFloatingPoint

    /// <summary>
    /// Returns the significand (mantissa) and exponent equal to <paramref name="value"/> according to the given <paramref name="numberBase"/>.<br/>
    /// The <paramref name="value"/> can be displayed in scientific notation with <c>"{significand} × {numberBase}^{exponent}"</c>.
    /// </summary>
    public static (BigReal significand, int exponent) GetSignificandAndExponent(BigReal value, int numberBase) {
        ArgumentOutOfRangeException.ThrowIfLessThan(numberBase, 2);

        if (IsZero(value)) {
            return (Zero, 0);
        }

        BigInteger bigNumberBase = numberBase;
        int exponent = 0;

        while (value >= bigNumberBase) {
            value /= bigNumberBase;
            exponent++;
        }

        while (value < One) {
            value *= bigNumberBase;
            exponent--;
        }

        return (value, exponent);
    }
    int IFloatingPoint<BigReal>.GetExponentByteCount() {
        throw new NotImplementedException();
    }
    int IFloatingPoint<BigReal>.GetExponentShortestBitLength() {
        throw new NotImplementedException();
    }
    int IFloatingPoint<BigReal>.GetSignificandBitLength() {
        throw new NotImplementedException();
    }
    int IFloatingPoint<BigReal>.GetSignificandByteCount() {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigReal>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigReal>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigReal>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
    bool IFloatingPoint<BigReal>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }

    #endregion

    #region IConvertible

    TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
    bool IConvertible.ToBoolean(IFormatProvider? provider) => !IsZero(this);
    sbyte IConvertible.ToSByte(IFormatProvider? provider) => (sbyte)this;
    byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;
    short IConvertible.ToInt16(IFormatProvider? provider) => (short)this;
    ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
    int IConvertible.ToInt32(IFormatProvider? provider) => (int)this;
    uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
    long IConvertible.ToInt64(IFormatProvider? provider) => (long)this;
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;
    char IConvertible.ToChar(IFormatProvider? provider) => (char)this;
    float IConvertible.ToSingle(IFormatProvider? provider) => (float)this;
    double IConvertible.ToDouble(IFormatProvider? provider) => (double)this;
    decimal IConvertible.ToDecimal(IFormatProvider? provider) => (decimal)this;
    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new InvalidCastException($"Cannot convert {nameof(BigReal)} to {nameof(DateTime)}");
    object IConvertible.ToType(Type conversionType, IFormatProvider? provider) {
        if (ReferenceEquals(conversionType, typeof(BigReal))) {
            return this;
        }
        else if (ReferenceEquals(conversionType, typeof(bool))) {
            return ((IConvertible)this).ToBoolean(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(sbyte))) {
            return ((IConvertible)this).ToSByte(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(byte))) {
            return ((IConvertible)this).ToByte(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(short))) {
            return ((IConvertible)this).ToInt16(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(ushort))) {
            return ((IConvertible)this).ToUInt16(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(int))) {
            return ((IConvertible)this).ToInt32(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(uint))) {
            return ((IConvertible)this).ToUInt32(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(long))) {
            return ((IConvertible)this).ToInt64(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(ulong))) {
            return ((IConvertible)this).ToUInt64(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(float))) {
            return ((IConvertible)this).ToSingle(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(double))) {
            return ((IConvertible)this).ToDouble(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(decimal))) {
            return ((IConvertible)this).ToDecimal(provider);
        }
        else if (ReferenceEquals(conversionType, typeof(DateTime))) {
            return ((IConvertible)this).ToDateTime(provider);
        }
        else {
            throw new InvalidCastException($"Cannot convert {nameof(BigReal)} to {conversionType.Name}");
        }
    }

    #endregion
}