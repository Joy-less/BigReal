using System;
using System.Numerics;
using Xunit.Abstractions;

namespace ExtendedNumerics.Tests;

public class BigRealTests(ITestOutputHelper Output) {
    private readonly ITestOutputHelper Output = Output;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TestToStringDigits(bool padDecimal) {
        for (int exp = 4; exp >= -4; exp--) {
            decimal testDigits = (decimal)(double.Pi * double.Pow(10.0, exp));
            Output.WriteLine(testDigits.ToString());

            BigReal bigFloat = new(testDigits);
            string str = bigFloat.ToString(100, padDecimal: padDecimal);
            Output.WriteLine(str);

            decimal compare = decimal.Parse(str);
            compare.ShouldBe(testDigits);
        }
    }

    [Fact]
    public void Signing() {
        new BigReal(new BigInteger(1), new BigInteger(1)).ShouldBe(1);
        new BigReal(new BigInteger(-1), new BigInteger(-1)).ShouldBe(1);

        new BigReal(new BigInteger(-1), new BigInteger(1)).ShouldBe(-1);
        new BigReal(new BigInteger(1), new BigInteger(-1)).ShouldBe(-1);
    }

    [Fact]
    public void Equality() {
        BigReal a = new(new BigInteger(1), new BigInteger(1));
        BigReal b = new(new BigInteger(2), new BigInteger(2));
        b.ShouldBe(a);

        BigReal c = new(new BigInteger(-1), new BigInteger(-1));
        c.ShouldBe(a);

        BigReal e = new(new BigInteger(-1), new BigInteger(1));
        BigReal f = new(new BigInteger(2), new BigInteger(-2));
        f.ShouldBe(e);
    }

    [Theory]
    [InlineData(1.5, 2)]
    [InlineData(2.5, 2)]
    [InlineData(-1.5, -2)]
    [InlineData(-2.5, -2)]
    public void RoundToEven(double input, double expected) {
        BigReal.Round(input, MidpointRounding.ToEven).ShouldBe(expected);
    }
    [Theory]
    [InlineData(12.34, 1, 12.3)]
    [InlineData(12.34, -1, 10)]
    public void RoundToDecimals(double input, int digits, double expected) {
        BigReal.Round(input, digits).ShouldBe(expected);
    }
    [Theory]
    [InlineData(1)]
    [InlineData(0.3)]
    [InlineData(-0.5)]
    public void Trigonometry(double value) {
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