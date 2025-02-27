using System;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

public class BigFloatUnitTest {
    private readonly ITestOutputHelper output;

    public BigFloatUnitTest(ITestOutputHelper output) {
        this.output = output;
    }

    [Fact]
    public void TestToStringDigits() {
        for (int exp = 4; exp >= -4; exp--) {
            decimal testDigits = (decimal)(Math.PI * Math.Pow(10.0, exp));
            output.WriteLine(testDigits.ToString());

            BigFloat bigFloat = new(testDigits);
            string str = bigFloat.ToString();
            output.WriteLine(str);

            decimal compare = decimal.Parse(str);
            Assert.Equal(testDigits, compare);
        }
    }

    [Fact]
    public void TestToStringZeroes() {
        for (int exp = 4; exp >= -4; exp--) {
            decimal testDigits = (decimal)(Math.Pow(10.0, exp));
            output.WriteLine(testDigits.ToString());

            BigFloat bigFloat = new(testDigits);
            string str = bigFloat.ToString(20, trailingPointZero: true);
            output.WriteLine(str);

            decimal compare = decimal.Parse(str);
            Assert.Equal(testDigits, compare);
        }
    }

    [Fact]
    public void Signing() {
        var a = new BigFloat(new BigInteger(1), new BigInteger(1));
        var b = new BigFloat(new BigInteger(-1), new BigInteger(-1));

        Assert.Equal(1, a);
        Assert.Equal(1, b);

        var c = new BigFloat(new BigInteger(-1), new BigInteger(1));
        var d = new BigFloat(new BigInteger(1), new BigInteger(-1));

        Assert.Equal(-1, c);
        Assert.Equal(-1, d);
    }

    [Fact]
    public void Equality() {
        var a = new BigFloat(new BigInteger(1), new BigInteger(1));
        var b = new BigFloat(new BigInteger(2), new BigInteger(2));
        Assert.Equal(b, a);

        var c = new BigFloat(new BigInteger(-1), new BigInteger(-1));
        Assert.Equal(c, a);

        var e = new BigFloat(new BigInteger(-1), new BigInteger(1));
        var f = new BigFloat(new BigInteger(2), new BigInteger(-2));
        Assert.Equal(e, f);
    }
}
