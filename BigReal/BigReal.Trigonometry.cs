using System.Numerics;

namespace ExtendedNumerics;

partial struct BigReal : ITrigonometricFunctions<BigReal> {
    /// <summary>
    /// Represents 15 decimal places of the natural logarithmic base, specified by the constant, e.
    /// </summary>
    /// <remarks>
    /// The exact value of this field is <c>2.718281828459045</c>.
    /// </remarks>
    public static BigReal E { get; } = new(543656365691809, 200000000000000);
    /// <summary>
    /// Represents 15 decimal places of the ratio of the circumference of a circle to its diameter, specified by the constant, π.
    /// </summary>
    /// <remarks>
    /// The exact value of this field is <c>3.141592653589793</c>.
    /// </remarks>
    public static BigReal Pi { get; } = new(3141592653589793, 1000000000000000);
    /// <summary>
    /// Represents 15 decimal places of the number of radians in one turn, specified by the constant, τ.
    /// </summary>
    /// <remarks>
    /// The exact value of this field is <c>6.283185307179586</c>.
    /// </remarks>
    public static BigReal Tau { get; } = new(3141592653589793, 500000000000000);

    /// <summary>
    /// Converts the given <paramref name="radians"/> (full turn = 2π) to degrees (full turn = 360).
    /// </summary>
    public static BigReal RadiansToDegrees(BigReal radians, int decimals = 15) {
        return radians * (180 / CalculatePi(decimals));
    }
    /// <summary>
    /// Converts the given <paramref name="radians"/> (full turn = 2π) to turns (full turn = 1).
    /// </summary>
    public static BigReal RadiansToTurns(BigReal radians, int decimals = 15) {
        return radians / CalculateTau(decimals);
    }
    /// <summary>
    /// Converts the given <paramref name="radians"/> (full turn = 2π) to gradians (full turn = 400).
    /// </summary>
    public static BigReal RadiansToGradians(BigReal radians, int decimals = 15) {
        return radians * (200 / CalculatePi(decimals));
    }
    /// <summary>
    /// Converts the given <paramref name="degrees"/> (full turn = 360) to radians (full turn = 2π).
    /// </summary>
    public static BigReal DegreesToRadians(BigReal degrees, int decimals = 15) {
        return degrees * (CalculatePi(decimals) / 180);
    }
    /// <summary>
    /// Converts the given <paramref name="degrees"/> (full turn = 360) to turns (full turn = 1).
    /// </summary>
    public static BigReal DegreesToTurns(BigReal degrees, int decimals = 15) {
        return degrees / 360;
    }
    /// <summary>
    /// Converts the given <paramref name="degrees"/> (full turn = 360) to gradians (full turn = 400).
    /// </summary>
    public static BigReal DegreesToGradians(BigReal degrees, int decimals = 15) {
        return degrees * new BigReal(10, 9);
    }
    /// <summary>
    /// Converts the given <paramref name="turns"/> (full turn = 1) to degrees (full turn = 360).
    /// </summary>
    public static BigReal TurnsToDegrees(BigReal turns, int decimals = 15) {
        return turns * 360;
    }
    /// <summary>
    /// Converts the given <paramref name="turns"/> (full turn = 1) to radians (full turn = 2π).
    /// </summary>
    public static BigReal TurnsToRadians(BigReal turns, int decimals = 15) {
        return turns * CalculateTau(decimals);
    }
    /// <summary>
    /// Converts the given <paramref name="turns"/> (full turn = 1) to gradians (full turn = 400).
    /// </summary>
    public static BigReal TurnsToGradians(BigReal turns, int decimals = 15) {
        return turns * 400;
    }
    /// <summary>
    /// Converts the given <paramref name="gradians"/> (full turn = 400) to degrees (full turn = 360).
    /// </summary>
    public static BigReal GradiansToDegrees(BigReal gradians, int decimals = 15) {
        return gradians * new BigReal(9, 10);
    }
    /// <summary>
    /// Converts the given <paramref name="gradians"/> (full turn = 400) to radians (full turn = 2π).
    /// </summary>
    public static BigReal GradiansToRadians(BigReal gradians, int decimals = 15) {
        return gradians * (CalculatePi(decimals) / 200);
    }
    /// <summary>
    /// Converts the given <paramref name="gradians"/> (full turn = 400) to turns (full turn = 1).
    /// </summary>
    public static BigReal GradiansToTurns(BigReal gradians, int decimals = 15) {
        return gradians / 400;
    }
    /// <summary>
    /// Returns the sine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Sin(double)"/> instead.
    /// </remarks>
    public static BigReal Sin(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
        }

        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/2284929
        BigReal cur = radians;
        BigReal acc = One;
        BigInteger fact = BigInteger.One;
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
    /// <remarks>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Cos(double)"/> instead.
    /// </remarks>
    public static BigReal Cos(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
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
    /// <b>Note:</b> The result is undefined when <paramref name="radians"/> is π/2 or 3π/2, so this method returns <see cref="NaN"/>.
    /// <br/><br/>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Tan(double)"/> instead.
    /// </remarks>
    public static BigReal Tan(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
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
    /// <b>Note:</b> The result is undefined when <paramref name="radians"/> is an odd multiple of π/2, so this method returns <see cref="NaN"/>.
    /// <br/><br/>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use 1/<see cref="double.Cos(double)"/> instead.
    /// </remarks>
    public static BigReal Sec(BigReal radians, int decimals = 15) {
        return One / Cos(radians, decimals);
    }
    /// <summary>
    /// Returns the secant of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> The result is undefined when <paramref name="radians"/> is a multiple of π, so this method returns <see cref="NaN"/>.
    /// <br/><br/>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use 1/<see cref="double.Sin(double)"/> instead.
    /// </remarks>
    public static BigReal Cosec(BigReal radians, int decimals = 15) {
        return One / Sin(radians, decimals);
    }
    /// <summary>
    /// Returns the cotangent of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> The result is undefined when <paramref name="radians"/> is a multiple of π, so this method returns <see cref="NaN"/>.
    /// <br/><br/>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Cos(double)"/>/<see cref="double.Sin(double)"/> instead.
    /// </remarks>
    public static BigReal Cot(BigReal radians, int decimals = 15) {
        return Cos(radians, decimals) / Sin(radians, decimals);
    }
    /// <summary>
    /// Returns the arc-sine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> This method throws an <see cref="ArgumentOutOfRangeException"/> when the magnitude of <paramref name="radians"/> is greater than one.
    /// <br/><br/>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Asin(double)"/> instead.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static BigReal Asin(BigReal radians, int decimals = 15) {
        if (radians < NegativeOne || radians > One) {
            throw new ArgumentOutOfRangeException(nameof(radians));
        }
        if (IsOne(radians)) {
            return CalculatePi(decimals) / 2;
        }
        if (IsNegativeOne(radians)) {
            return -CalculatePi(decimals) / 2;
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
    /// <b>Note:</b> This method throws an <see cref="ArgumentOutOfRangeException"/> when the magnitude of <paramref name="radians"/> is greater than one.
    /// <br/><br/>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Acos(double)"/> instead.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static BigReal Acos(BigReal radians, int decimals = 15) {
        if (radians < NegativeOne || radians > One) {
            throw new ArgumentOutOfRangeException(nameof(radians));
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
    /// <remarks>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Atan(double)"/> instead.
    /// </remarks>
    public static BigReal Atan(BigReal radians, int decimals = 15) {
        if (!IsFinite(radians)) {
            return NaN;
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
    /// <remarks>
    /// <b>Warning:</b> This method may be extremely slow. If possible, use <see cref="double.Atan2(double, double)"/> instead.
    /// </remarks>
    public static BigReal Atan2(BigReal y, BigReal x, int decimals = 15) {
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
        if (decimals <= 15) {
            return E;
        }

        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/43564002
        BigReal result = One;
        BigReal factorial = One;
        BigReal term;
        int n = 1;
        while (true) {
            factorial *= n;
            term = One / factorial;
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
        if (decimals <= 15) {
            return Pi;
        }

        const int MaxStackallocBytes = 256;

        // https://stackoverflow.com/a/11679007
        decimals += 2;

        int bufferLength = (decimals * 10 / 3) + 2;
        Span<uint> dividend = (bufferLength <= MaxStackallocBytes / sizeof(uint)) ? stackalloc uint[bufferLength] : new uint[bufferLength];
        Span<uint> remainder = (bufferLength <= MaxStackallocBytes / sizeof(uint)) ? stackalloc uint[bufferLength] : new uint[bufferLength];

        Span<uint> digits = (decimals <= MaxStackallocBytes / sizeof(uint)) ? stackalloc uint[decimals] : new uint[decimals];

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
        if (decimals <= 15) {
            return Tau;
        }

        return CalculatePi(decimals) * 2;
    }
}