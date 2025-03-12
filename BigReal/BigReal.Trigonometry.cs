using System.Numerics;

namespace ExtendedNumerics;

partial struct BigReal : ITrigonometricFunctions<BigReal> {
    /// <summary>
    /// Represents 15 digits of the natural logarithmic base, specified by the constant, e.
    /// </summary>
    public static BigReal E { get; } = double.E;
    /// <summary>
    /// Represents 15 digits of the ratio of the circumference of a circle to its diameter, specified by the constant, π.
    /// </summary>
    public static BigReal Pi { get; } = double.Pi;
    /// <summary>
    /// Represents 15 digits of the number of radians in one turn, specified by the constant, τ.
    /// </summary>
    public static BigReal Tau { get; } = double.Tau;

    /// <summary>
    /// Converts the given <paramref name="radians"/> to degrees.
    /// </summary>
    public static BigReal RadiansToDegrees(BigReal radians) {
        return radians * (180 / Pi);
    }
    /// <summary>
    /// Converts the given <paramref name="degrees"/> to radians.
    /// </summary>
    public static BigReal DegreesToRadians(BigReal degrees) {
        return degrees * (Pi / 180);
    }
    /// <summary>
    /// Returns the sine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Sin(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
        }
        if (TryCalculateAsDouble(radians, double.Sin, decimals, out double result)) {
            return result;
        }

        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/2284929
        BigReal cur = radians;
        BigReal acc = 1;
        BigInteger fact = 1;
        BigReal pow = radians;
        int n = 1;
        while (true) {
            fact *= (2 * n) * ((2 * n) + 1);
            pow *= -1 * radians * radians;
            acc = pow / fact;
            cur += acc;
            n++;
            if (Abs(acc) <= epsilon) {
                break;
            }
        }
        return cur;
    }
    static BigReal ITrigonometricFunctions<BigReal>.Sin(BigReal radians) {
        return Sin(radians);
    }
    static BigReal ITrigonometricFunctions<BigReal>.SinPi(BigReal radians) {
        return Sin(radians * Pi);
    }
    /// <summary>
    /// Returns the cosine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Cos(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
        }
        if (TryCalculateAsDouble(radians, double.Cos, decimals, out double result)) {
            return result;
        }

        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/2284969
        BigReal s = One;
        BigReal t = One;
        int n = 0;
        while (true) {
            n++;
            t = (-t * radians * radians) / ((2 * n - 1) * (2 * n));
            s += t;
            if (Abs(t / s) <= epsilon) {
                break;
            }
        }
        return s;
    }
    static BigReal ITrigonometricFunctions<BigReal>.Cos(BigReal radians) {
        return Cos(radians);
    }
    static BigReal ITrigonometricFunctions<BigReal>.CosPi(BigReal radians) {
        return Cos(radians * Pi);
    }
    /// <summary>
    /// Returns the tangent of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is π/2 or 3π/2.
    /// </remarks>
    public static BigReal Tan(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
        }
        if (TryCalculateAsDouble(radians, double.Tan, decimals, out double result)) {
            return result;
        }
        return Sin(radians, decimals) / Cos(radians, decimals);
    }
    static BigReal ITrigonometricFunctions<BigReal>.Tan(BigReal radians) {
        return Tan(radians);
    }
    static BigReal ITrigonometricFunctions<BigReal>.TanPi(BigReal radians) {
        return Tan(radians * Pi);
    }
    static (BigReal Sin, BigReal Cos) ITrigonometricFunctions<BigReal>.SinCos(BigReal radians) {
        return (Sin(radians), Cos(radians));
    }
    static (BigReal SinPi, BigReal CosPi) ITrigonometricFunctions<BigReal>.SinCosPi(BigReal radians) {
        radians *= Pi;
        return (Sin(radians), Cos(radians));
    }
    /// <summary>
    /// Returns the secant of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is an odd multiple of π/2.
    /// </remarks>
    public static BigReal Sec(BigReal radians, int decimals = 15) {
        return One / Cos(radians, decimals);
    }
    /// <summary>
    /// Returns the secant of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is a multiple of π.
    /// </remarks>
    public static BigReal Cosec(BigReal radians, int decimals = 15) {
        return One / Sin(radians, decimals);
    }
    /// <summary>
    /// Returns the cotangent of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is a multiple of π.
    /// </remarks>
    public static BigReal Cot(BigReal radians, int decimals = 15) {
        return Cos(radians, decimals) / Sin(radians, decimals);
    }
    /// <summary>
    /// Returns the arc-sine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result throws when the magnitude of <paramref name="radians"/> is greater than one.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static BigReal Asin(BigReal radians, int decimals = 15) {
        if (radians < NegativeOne || radians > One) {
            throw new ArgumentOutOfRangeException(nameof(radians));
        }
        if (IsOne(radians)) {
            return Pi / 2;
        }
        if (IsNegativeOne(radians)) {
            return -Pi / 2;
        }
        if (TryCalculateAsDouble(radians, double.Asin, decimals, out double result)) {
            return result;
        }

        // https://stackoverflow.com/a/7380529
        return Atan2(radians, Sqrt((One + radians) * (One - radians)), decimals);
    }
    static BigReal ITrigonometricFunctions<BigReal>.Asin(BigReal radians) {
        return Asin(radians);
    }
    static BigReal ITrigonometricFunctions<BigReal>.AsinPi(BigReal radians) {
        return Asin(radians * Pi);
    }
    /// <summary>
    /// Returns the arc-cosine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result throws when the magnitude of <paramref name="radians"/> is greater than one.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static BigReal Acos(BigReal radians, int decimals = 15) {
        if (radians < NegativeOne || radians > One) {
            throw new ArgumentOutOfRangeException(nameof(radians));
        }
        if (TryCalculateAsDouble(radians, double.Acos, decimals, out double result)) {
            return result;
        }

        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/7380529
        return Atan2(Sqrt((One + radians) * (One - radians), decimals), radians, decimals);
    }
    static BigReal ITrigonometricFunctions<BigReal>.Acos(BigReal radians) {
        return Acos(radians);
    }
    static BigReal ITrigonometricFunctions<BigReal>.AcosPi(BigReal radians) {
        return Acos(radians * Pi);
    }
    /// <summary>
    /// Returns the arc-tangent of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Atan(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
        }
        if (TryCalculateAsDouble(radians, double.Atan, decimals, out double result)) {
            return result;
        }

        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/40077756
        BigReal sum = 0;
        BigReal term = radians;
        int n = 0;
        while (true) {
            term = (Pow(radians, (2 * n) + 1) * Pow(-1, n)) / ((2 * n) + 1);
            sum += term;
            n++;
            if (Abs(term) <= epsilon) {
                break;
            }
        }
        return sum;
    }
    static BigReal ITrigonometricFunctions<BigReal>.Atan(BigReal radians) {
        return Atan(radians);
    }
    static BigReal ITrigonometricFunctions<BigReal>.AtanPi(BigReal radians) {
        return Atan(radians * Pi);
    }
    /// <summary>
    /// Returns the arc-tangent of the quotient of <paramref name="y"/> and <paramref name="x"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Atan2(BigReal y, BigReal x, int decimals = 15) {
        if (TryCalculateAsDouble(y, x, double.Atan2, decimals, out double result)) {
            return result;
        }

        // https://en.wikipedia.org/wiki/Atan2#Definition
        if (x > Zero) {
            return Atan(y / x, decimals);
        }
        else if (x < Zero && y >= Zero) {
            return Atan(y / x, decimals) + CalculatePi(decimals);
        }
        else if (x < Zero && y < Zero) {
            return Atan(y / x, decimals) - CalculatePi(decimals);
        }
        else if (x == Zero && y > Zero) {
            return CalculatePi(decimals) / 2;
        }
        else if (x == Zero && y < Zero) {
            return -CalculatePi(decimals) / 2;
        }
        else {
            return NaN;
        }
    }
    /// <summary>
    /// Returns e, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal CalculateE(int decimals) {
        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/43564002
        BigReal result = 1;
        BigReal factorial = 1;
        BigReal term;
        int n = 1;
        while (true) {
            factorial *= n;
            term = 1m / factorial;
            result += term;
            n++;
            if (Abs(term) <= epsilon) {
                break;
            }
        }
        return result;
    }
    /// <summary>
    /// Returns π, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal CalculatePi(int decimals) {
        const int MaxStackallocLength = 256 / sizeof(uint);

        // https://stackoverflow.com/a/11679007
        decimals += 2;

        int bufferLength = (decimals * 10 / 3) + 2;
        Span<uint> dividend = bufferLength <= MaxStackallocLength ? stackalloc uint[bufferLength] : new uint[bufferLength];
        Span<uint> remainder = bufferLength <= MaxStackallocLength ? stackalloc uint[bufferLength] : new uint[bufferLength];

        Span<uint> digits = decimals <= MaxStackallocLength ? stackalloc uint[decimals] : new uint[decimals];

        for (int j = 0; j < dividend.Length; j++) {
            dividend[j] = 20;
        }

        for (int i = 0; i < decimals; i++) {
            uint carry = 0;
            for (int j = 0; j < dividend.Length; j++) {
                uint numerator = (uint)(dividend.Length - j - 1);
                uint denominator = (numerator * 2) + 1;

                dividend[j] += carry;

                uint quotient = dividend[j] / denominator;
                remainder[j] = dividend[j] % denominator;

                carry = quotient * numerator;
            }

            digits[i] = dividend[^1] / 10;

            remainder[dividend.Length - 1] = dividend[^1] % 10;

            for (int j = 0; j < dividend.Length; j++) {
                dividend[j] = remainder[j] * 10;
            }
        }

        BigInteger resultDigits = 0;
        uint carryOver = 0;

        for (int i = digits.Length - 1; i >= 0; i--) {
            digits[i] += carryOver;
            carryOver = digits[i] / 10;

            BigInteger columnMagnitude = BigInteger.Pow(10, digits.Length - i - 1);
            resultDigits += (digits[i] % 10) * columnMagnitude;
        }

        BigReal result = new BigReal(resultDigits) / BigInteger.Pow(10, decimals - 1);
        return result;
    }
    /// <summary>
    /// Returns τ, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal CalculateTau(int decimals) {
        return CalculatePi(decimals) * 2;
    }
}