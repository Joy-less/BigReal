using System;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;
using BigFloatSharp;

namespace UnitTests;

public class BigFloatUnitTest(ITestOutputHelper Output) {
    private readonly ITestOutputHelper Output = Output;

    [Fact]
    public void TestToStringDigits() {
        for (int exp = 4; exp >= -4; exp--) {
            decimal testDigits = (decimal)(Math.PI * Math.Pow(10.0, exp));
            Output.WriteLine(testDigits.ToString());

            BigFloat bigFloat = new(testDigits);
            string str = bigFloat.ToString();
            Output.WriteLine(str);

            decimal compare = decimal.Parse(str);
            Assert.Equal(testDigits, compare);
        }
    }

    [Fact]
    public void TestToStringZeroes() {
        for (int exp = 4; exp >= -4; exp--) {
            decimal testDigits = (decimal)(Math.Pow(10.0, exp));
            Output.WriteLine(testDigits.ToString());

            BigFloat bigFloat = new(testDigits);
            string str = bigFloat.ToString(20, padDecimal: true);
            Output.WriteLine(str);

            decimal compare = decimal.Parse(str);
            Assert.Equal(testDigits, compare);
        }
    }

    [Fact]
    public void Signing() {
        BigFloat a = new(new BigInteger(1), new BigInteger(1));
        BigFloat b = new(new BigInteger(-1), new BigInteger(-1));

        Assert.Equal(1, a);
        Assert.Equal(1, b);

        BigFloat c = new(new BigInteger(-1), new BigInteger(1));
        BigFloat d = new(new BigInteger(1), new BigInteger(-1));

        Assert.Equal(-1, c);
        Assert.Equal(-1, d);
    }

    [Fact]
    public void Equality() {
        BigFloat a = new(new BigInteger(1), new BigInteger(1));
        BigFloat b = new(new BigInteger(2), new BigInteger(2));
        Assert.Equal(b, a);

        BigFloat c = new(new BigInteger(-1), new BigInteger(-1));
        Assert.Equal(c, a);

        BigFloat e = new(new BigInteger(-1), new BigInteger(1));
        BigFloat f = new(new BigInteger(2), new BigInteger(-2));
        Assert.Equal(e, f);
    }
}
