using System.Numerics;

namespace ExtendedNumerics;

partial struct BigReal : ITrigonometricFunctions<BigReal> {
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
        // https://stackoverflow.com/a/11679007
        decimals++;

        Span<uint> x = new uint[decimals * 10 / 3 + 2];
        Span<uint> r = new uint[decimals * 10 / 3 + 2];

        Span<uint> pi = new uint[decimals];

        for (int j = 0; j < x.Length; j++) {
            x[j] = 20;
        }

        for (int i = 0; i < decimals; i++) {
            uint carry = 0;
            for (int j = 0; j < x.Length; j++) {
                uint num = (uint)(x.Length - j - 1);
                uint dem = num * 2 + 1;

                x[j] += carry;

                uint q = x[j] / dem;
                r[j] = x[j] % dem;

                carry = q * num;
            }

            pi[i] = x[^1] / 10;

            r[x.Length - 1] = x[^1] % 10; ;

            for (int j = 0; j < x.Length; j++) {
                x[j] = r[j] * 10;
            }
        }

        BigReal result = Zero;
        uint c = 0;

        for (int i = pi.Length - 1; i >= 0; i--) {
            pi[i] += c;
            c = pi[i] / 10;

            BigInteger columnMagnitude = BigInteger.Pow(10, pi.Length - i - 1);
            result = Multiply(pi[i] % 10, columnMagnitude) + result;
        }
        return result / BigInteger.Pow(10, decimals - 1);
    }
    /// <summary>
    /// Returns τ, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal CalculateTau(int decimals) {
        return CalculatePi(decimals) * 2;
    }
}