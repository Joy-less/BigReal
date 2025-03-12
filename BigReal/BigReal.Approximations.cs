namespace ExtendedNumerics;

partial struct BigReal {
    /// <summary>
    /// The number of reliable decimal places in <see cref="float"/>.
    /// </summary>
    private const int SingleReliableDecimals = 6;
    /// <summary>
    /// The number of reliable decimal places in <see cref="double"/>.
    /// </summary>
    private const int DoubleReliableDecimals = 15;
    /// <summary>
    /// The number of reliable decimal places in <see cref="decimal"/>.
    /// </summary>
    private const int DecimalReliableDecimals = 28;

    /// <summary>
    /// Performs a calculation by converting the input to <see cref="double"/> if within range and precision.
    /// </summary>
    private static bool TryCalculateAsDouble(BigReal input, Func<double, double> calculate, int decimals, out double result) {
        if (decimals <= DoubleReliableDecimals && IsInRangeOf<double>(input)) {
            result = calculate((double)input);
            result = double.Round(result, decimals, MidpointRounding.AwayFromZero);
            return true;
        }
        result = default;
        return false;
    }
    /// <inheritdoc cref="TryCalculateAsDouble(BigReal, Func{double, double}, int, out double)"/>
    private static bool TryCalculateAsDouble(BigReal input1, BigReal input2, Func<double, double, double> calculate, int decimals, out double result) {
        if (decimals <= DoubleReliableDecimals && IsInRangeOf<double>(input1) && IsInRangeOf<double>(input2)) {
            result = calculate((double)input1, (double)input2);
            result = double.Round(result, decimals, MidpointRounding.AwayFromZero);
            return true;
        }
        result = default;
        return false;
    }
    /// <inheritdoc cref="TryCalculateAsDouble(BigReal, Func{double, double}, int, out double)"/>
    private static bool TryCalculateAsDouble(BigReal input1, int input2, Func<double, int, double> calculate, int decimals, out double result) {
        if (decimals <= DoubleReliableDecimals && IsInRangeOf<double>(input1)) {
            result = calculate((double)input1, input2);
            result = double.Round(result, decimals, MidpointRounding.AwayFromZero);
            return true;
        }
        result = default;
        return false;
    }
}