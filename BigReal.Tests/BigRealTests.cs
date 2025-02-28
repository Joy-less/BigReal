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
}
