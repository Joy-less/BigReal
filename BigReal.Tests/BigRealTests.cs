using System;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;
using ExtendedNumerics;

namespace UnitTests;

public class BigRealTests(ITestOutputHelper Output) {
    private readonly ITestOutputHelper Output = Output;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TestToStringDigits(bool padDecimal) {
        for (int exp = 4; exp >= -4; exp--) {
            decimal testDigits = (decimal)(Math.PI * Math.Pow(10.0, exp));
            Output.WriteLine(testDigits.ToString());

            BigReal bigFloat = new(testDigits);
            string str = bigFloat.ToString(100, padDecimal: padDecimal);
            Output.WriteLine(str);

            decimal compare = decimal.Parse(str);
            Assert.Equal(testDigits, compare);
        }
    }

    [Fact]
    public void Signing() {
        BigReal a = new(new BigInteger(1), new BigInteger(1));
        BigReal b = new(new BigInteger(-1), new BigInteger(-1));

        Assert.Equal(1, a);
        Assert.Equal(1, b);

        BigReal c = new(new BigInteger(-1), new BigInteger(1));
        BigReal d = new(new BigInteger(1), new BigInteger(-1));

        Assert.Equal(-1, c);
        Assert.Equal(-1, d);
    }

    [Fact]
    public void Equality() {
        BigReal a = new(new BigInteger(1), new BigInteger(1));
        BigReal b = new(new BigInteger(2), new BigInteger(2));
        Assert.Equal(b, a);

        BigReal c = new(new BigInteger(-1), new BigInteger(-1));
        Assert.Equal(c, a);

        BigReal e = new(new BigInteger(-1), new BigInteger(1));
        BigReal f = new(new BigInteger(2), new BigInteger(-2));
        Assert.Equal(e, f);
    }

    [Theory]
    [InlineData(1.5, 2)]
    [InlineData(2.5, 2)]
    [InlineData(-1.5, -2)]
    [InlineData(-2.5, -2)]
    public void RoundToEven(double input, double expected) {
        Assert.Equal(expected, BigReal.Round(input, MidpointRounding.ToEven));
    }
    [Theory]
    [InlineData(12.34, 1, 12.3)]
    [InlineData(12.34, -1, 10)]
    public void RoundToDecimals(double input, int digits, double expected) {
        Assert.Equal(expected, BigReal.Round(input, digits));
    }
    [Theory]
    [InlineData(1)]
    [InlineData(0.3)]
    [InlineData(-0.5)]
    public void Trigonometry(double value) {
        AssertApproximateEqual(double.Sin(value), BigReal.Sin(value));
        AssertApproximateEqual(double.Cos(value), BigReal.Cos(value));
        AssertApproximateEqual(double.Tan(value), BigReal.Tan(value));
        AssertApproximateEqual(1 / double.Sin(value), BigReal.Cosec(value));
        AssertApproximateEqual(1 / double.Cos(value), BigReal.Sec(value));
        AssertApproximateEqual(1 / double.Tan(value), BigReal.Cot(value));
        AssertApproximateEqual(double.Asin(value), BigReal.Asin(value));
        AssertApproximateEqual(double.Acos(value), BigReal.Acos(value));
        AssertApproximateEqual(double.Atan(value), BigReal.Atan(value));
        AssertApproximateEqual(double.Atan2(value, value), BigReal.Atan2(value, value));
    }

    private static void AssertApproximateEqual(BigReal a, BigReal b, int decimals = 2) {
        BigReal delta = BigReal.Abs(a - b);
        BigReal epsilon = BigReal.One / BigReal.Pow(BigReal.Ten, decimals);
        Assert.True(delta < epsilon, $"{delta} > {epsilon}");
    }
}