using System;
using System.Numerics;
using Xunit.Abstractions;

namespace ExtendedNumerics.Tests;

public class BigRealTests(ITestOutputHelper output) {
    private readonly ITestOutputHelper Output = output;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToStringDigitsTest(bool padDecimal) {
        for (int exp = 4; exp >= -4; exp--) {
            decimal testDigits = (decimal)(double.Pi * double.Pow(10.0, exp));
            Output.WriteLine(testDigits.ToString());

            BigReal bigReal = new(testDigits);
            string str = bigReal.ToString(100, padDecimal: padDecimal);
            Output.WriteLine(str);

            decimal compare = decimal.Parse(str);
            compare.ShouldBe(testDigits);
        }
    }

    [Fact]
    public void SigningTest() {
        new BigReal(new BigInteger(1), new BigInteger(1)).ShouldBe(1);
        new BigReal(new BigInteger(-1), new BigInteger(-1)).ShouldBe(1);

        new BigReal(new BigInteger(-1), new BigInteger(1)).ShouldBe(-1);
        new BigReal(new BigInteger(1), new BigInteger(-1)).ShouldBe(-1);
    }

    [Fact]
    public void EqualityTest() {
        BigReal a = new(new BigInteger(1), new BigInteger(1));
        BigReal b = new(new BigInteger(2), new BigInteger(2));
        b.ShouldBe(a);

        BigReal c = new(new BigInteger(-1), new BigInteger(-1));
        c.ShouldBe(a);

        BigReal e = new(new BigInteger(-1), new BigInteger(1));
        BigReal f = new(new BigInteger(2), new BigInteger(-2));
        f.ShouldBe(e);

        BigReal g = BigReal.PositiveInfinity;
        BigReal h = BigReal.NaN;
        g.Equals(h).ShouldBeFalse();
        (g == h).ShouldBeFalse();

        BigReal i = BigReal.NaN;
        BigReal j = BigReal.NaN;
        i.Equals(j).ShouldBeTrue();
        (i == j).ShouldBeFalse();

        BigReal k = BigReal.PositiveInfinity;
        BigReal l = BigReal.PositiveInfinity;
        k.Equals(l).ShouldBeTrue();
        (k == l).ShouldBeTrue();

        BigReal m = BigReal.PositiveInfinity;
        BigReal n = BigReal.NegativeInfinity;
        m.Equals(n).ShouldBeFalse();
        (m == n).ShouldBeFalse();
    }

    [Theory]
    [InlineData(1.5, 2)]
    [InlineData(2.5, 2)]
    [InlineData(-1.5, -2)]
    [InlineData(-2.5, -2)]
    public void RoundToEvenTest(double input, double expected) {
        ShouldBeApproximatelyEqual(BigReal.Round(input, MidpointRounding.ToEven), expected);
    }
    [Theory]
    [InlineData(12.34, 1, 12.3)]
    [InlineData(12.34, -1, 10)]
    public void RoundToDecimalsTest(double input, int digits, double expected) {
        ShouldBeApproximatelyEqual(BigReal.Round(input, digits), expected);
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

    private static void ShouldBeApproximatelyEqual(BigReal a, BigReal b, int decimals = 15) {
        BigReal delta = BigReal.Abs(a - b);
        BigReal epsilon = BigReal.One / BigReal.Pow(BigReal.Ten, decimals);
        (delta < epsilon).ShouldBeTrue($"{delta} > {epsilon}");
    }
}