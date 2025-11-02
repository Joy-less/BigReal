using System;
using System.Numerics;

namespace ExtendedNumerics.Tests;

public class BigRealTests {
    [Fact]
    public void BasicMathTest() {
        BigReal three = new(3);
        BigReal seven = new(7);

        BigReal.Add(three, seven).ShouldBe(10);
        (three + seven).ShouldBe(10);

        BigReal.Subtract(three, seven).ShouldBe(-4);
        (three - seven).ShouldBe(-4);

        BigReal.Multiply(three, seven).ShouldBe(21);
        (three * seven).ShouldBe(21);

        BigReal.Divide(three, seven).ShouldBe(new BigReal(3, 7));
        (three / seven).ShouldBe(new BigReal(3, 7));

        BigReal.Remainder(three, seven).ShouldBe(3);
        (three % seven).ShouldBe(3);

        BigReal.DivRem(three, seven).ShouldBe((new BigReal(3, 7), 3));
    }
    [Fact]
    public void PowRootTest() {
        BigReal three = new(3);
        BigReal seven = new(7);

        BigReal.Pow(three, seven).ShouldBe(2187);
        ShouldBeApproximatelyEqual(BigReal.Pow(three, -seven), 0.000457, 6);

        ShouldBeApproximatelyEqual(BigReal.Sqrt(three), 1.732, 3);
        ShouldBeApproximatelyEqual(BigReal.Sqrt(three, decimals: 40), 1.732, 3); // Slow
        ShouldBeApproximatelyEqual(BigReal.Cbrt(three), 1.442, 3);
        ShouldBeApproximatelyEqual(BigReal.Cbrt(three, decimals: 40), 1.442, 3); // Slow

        ShouldBeApproximatelyEqual(BigReal.RootN(three, (int)seven), 1.16993, 5);
        ShouldBeApproximatelyEqual(BigReal.RootN(three, (int)seven, decimals: 40), 1.16993, 5); // Slow
        ShouldBeApproximatelyEqual(BigReal.Pow(three, 1 / seven), 1.16993, 5);
        ShouldBeApproximatelyEqual(BigReal.Pow(three, 1 / seven, decimals: 40), 1.16993, 5); // Slow
    }
    [Fact]
    public void ExpTest() {
        ShouldBeApproximatelyEqual(BigReal.Exp(5.3), 200.33681, 5);
        ShouldBeApproximatelyEqual(BigReal.Exp(5.3, decimals: 40), 200.33681, 3); // Slow (slightly)
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToStringTest(bool padDecimal) {
        for (int exp = 4; exp >= -4; exp--) {
            double originalDouble = double.Pi * double.Pow(10.0, exp);
            BigReal asBigReal = new(originalDouble);
            double asDouble = double.Parse(asBigReal.ToString(100, padDecimal: padDecimal));
            originalDouble.ShouldBe(asDouble);
        }
    }
    [Fact]
    public void SignTest() {
        new BigReal(1, 1).ShouldBe(1);
        new BigReal(-1, -1).ShouldBe(1);

        new BigReal(-1, 1).ShouldBe(-1);
        new BigReal(1, -1).ShouldBe(-1);
    }
    [Fact]
    public void FiniteEqualityTest() {
        BigReal oneOverOne = new(new BigInteger(1), new BigInteger(1));
        BigReal twoOverTwo = new(new BigInteger(2), new BigInteger(2));
        oneOverOne.Equals(twoOverTwo).ShouldBeTrue();
        (oneOverOne == twoOverTwo).ShouldBeTrue();

        BigReal minusOneOverMinusOne = new(new BigInteger(-1), new BigInteger(-1));
        oneOverOne.Equals(minusOneOverMinusOne).ShouldBeTrue();
        (oneOverOne == minusOneOverMinusOne).ShouldBeTrue();

        BigReal minusOneOverOne = new(new BigInteger(-1), new BigInteger(1));
        BigReal twoOverMinusTwo = new(new BigInteger(2), new BigInteger(-2));
        twoOverMinusTwo.ShouldBe(minusOneOverOne);
    }
    [Fact]
    public void NonFiniteEqualityTest() {
        BigReal nan = BigReal.NaN;
        BigReal nan2 = BigReal.NaN;
        nan.Equals(nan2).ShouldBeTrue();
        (nan == nan2).ShouldBeFalse();

        BigReal positiveInfinity = BigReal.PositiveInfinity;
        nan.Equals(positiveInfinity).ShouldBeFalse();
        (nan == positiveInfinity).ShouldBeFalse();

        BigReal positiveInfinity2 = BigReal.PositiveInfinity;
        positiveInfinity.Equals(positiveInfinity2).ShouldBeTrue();
        (positiveInfinity == positiveInfinity2).ShouldBeTrue();

        BigReal negativeInfinity = BigReal.NegativeInfinity;
        nan.Equals(negativeInfinity).ShouldBeFalse();
        positiveInfinity.Equals(negativeInfinity).ShouldBeFalse();
        (nan == negativeInfinity).ShouldBeFalse();
        (positiveInfinity == negativeInfinity).ShouldBeFalse();
    }
    [Theory]
    [InlineData(1.5, 2)]
    [InlineData(2.5, 2)]
    [InlineData(-1.5, -2)]
    [InlineData(-2.5, -2)]
    public void RoundToEvenTest(double input, double expected) {
        ShouldBeApproximatelyEqual(BigReal.Round((BigReal)input, MidpointRounding.ToEven), expected);
    }
    [Theory]
    [InlineData(12.34, 1, 12.3)]
    [InlineData(12.34, -1, 10)]
    public void RoundToDecimalsTest(double input, int decimals, double expected) {
        ShouldBeApproximatelyEqual(BigReal.Round((BigReal)input, decimals), expected);
    }
    [Theory]
    [InlineData(1)]
    [InlineData(0.3)]
    [InlineData(-0.5)]
    public void TrigonometryTest(double value) {
        ShouldBeApproximatelyEqual(double.Sin(value), BigReal.Sin(value));
        ShouldBeApproximatelyEqual(double.Cos(value), BigReal.Cos(value));
        ShouldBeApproximatelyEqual(double.Tan(value), BigReal.Tan(value));
        ShouldBeApproximatelyEqual(1 / double.Sin(value), BigReal.Cosec(value));
        ShouldBeApproximatelyEqual(1 / double.Cos(value), BigReal.Sec(value));
        ShouldBeApproximatelyEqual(1 / double.Tan(value), BigReal.Cot(value));
        ShouldBeApproximatelyEqual(double.Asin(value), BigReal.Asin(value));
        ShouldBeApproximatelyEqual(double.Acos(value), BigReal.Acos(value));
        ShouldBeApproximatelyEqual(double.Atan(value), BigReal.Atan(value));
        ShouldBeApproximatelyEqual(double.Atan2(value, value), BigReal.Atan2(value, value));
        ShouldBeApproximatelyEqual(double.E, BigReal.CalculateE(15));
        ShouldBeApproximatelyEqual(double.Pi, BigReal.CalculatePi(15));
        ShouldBeApproximatelyEqual(double.Tau, BigReal.CalculateTau(15));
    }

    private static void ShouldBeApproximatelyEqual(BigReal actual, BigReal expected, int decimals = 15) {
        BigReal delta = BigReal.Abs(actual - expected);
        BigReal epsilon = BigReal.One / BigReal.Pow(BigReal.Ten, decimals);
        (delta < epsilon).ShouldBeTrue($"{actual} != {expected} ({delta} > {epsilon})");
    }
}