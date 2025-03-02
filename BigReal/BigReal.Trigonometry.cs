using System.Numerics;

namespace ExtendedNumerics;

partial struct BigReal {
    /// <summary>
    /// Converts the given <paramref name="radians"/> to degrees.
    /// </summary>
    public static BigReal RadToDeg(BigReal radians) {
        return radians * (180 / Pi);
    }
    /// <summary>
    /// Converts the given <paramref name="degrees"/> to radians.
    /// </summary>
    public static BigReal DegToRad(BigReal degrees) {
        return degrees * (Pi / 180);
    }
    /// <summary>
    /// Returns the sine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Sin(BigReal radians, int decimals = 20) {
        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/2284929
        int i = 1;
        BigReal cur = radians;
        BigReal acc = 1;
        BigInteger fact = 1;
        BigReal pow = radians;
        while (Abs(acc) > epsilon) {
            fact *= (2 * i) * ((2 * i) + 1);
            pow *= -1 * radians * radians;
            acc = pow / fact;
            cur += acc;
            i++;
        }
        return cur;
    }
    /// <summary>
    /// Returns the cosine of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    public static BigReal Cos(BigReal radians, int decimals = 20) {
        // Convert decimals to epsilon (e.g. 3 -> 0.001)
        BigReal epsilon = One / Pow(Ten, decimals);

        // https://stackoverflow.com/a/2284969
        int p = 0;
        BigReal s = 1;
        BigReal t = 1;
        while (Abs(t / s) > epsilon) {
            p++;
            t = (-t * radians * radians) / ((2 * p - 1) * (2 * p));
            s += t;
        }
        return s;
    }
    /// <summary>
    /// Returns the tangent of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is π/2 or 3π/2.
    /// </remarks>
    public static BigReal Tan(BigReal radians, int decimals = 20) {
        return Sin(radians, decimals) / Cos(radians, decimals);
    }
    /// <summary>
    /// Returns the secant of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is an odd multiple of π/2.
    /// </remarks>
    public static BigReal Sec(BigReal radians, int decimals = 20) {
        return One / Cos(radians, decimals);
    }
    /// <summary>
    /// Returns the secant of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is a multiple of π.
    /// </remarks>
    public static BigReal Cosec(BigReal radians, int decimals = 20) {
        return One / Sin(radians, decimals);
    }
    /// <summary>
    /// Returns the cotangent of <paramref name="radians"/>, correct to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// The result is undefined when <paramref name="radians"/> is a multiple of π.
    /// </remarks>
    public static BigReal Cot(BigReal radians, int decimals = 20) {
        return Cos(radians, decimals) / Sin(radians, decimals);
    }
}