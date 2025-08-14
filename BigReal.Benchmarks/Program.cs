using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Globalization;
using System.Numerics;

namespace ExtendedNumerics.Benchmarks;

public class Program {
    public static void Main() {
        BenchmarkSwitcher.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()).Run();
    }
}

[MemoryDiagnoser]
public class LibraryBenchmarks {
    [Benchmark]
    public void Add_BigReal() {
        BigReal result = (BigReal)10 + (BigReal)3;
    }
    [Benchmark]
    public void Add_BigDecimal() {
        BigDecimal result = (BigDecimal)10 + (BigDecimal)3;
    }
    [Benchmark]
    public void Add_BigFloat() {
        BigFloat result = (BigFloat)10 + (BigFloat)3;
    }

    [Benchmark]
    public void Divide_BigReal() {
        BigReal result = (BigReal)10 / (BigReal)3;
    }
    [Benchmark]
    public void Divide_BigDecimal() {
        BigDecimal result = (BigDecimal)10 / (BigDecimal)3;
    }
    [Benchmark]
    public void Divide_BigFloat() {
        BigFloat result = (BigFloat)10 / (BigFloat)3;
    }

    [Benchmark]
    public void DivideToString_BigReal() {
        BigReal result = (BigReal)10 / (BigReal)3;
        result.ToString();
    }
    [Benchmark]
    public void DivideToString_BigDecimal() {
        BigDecimal result = (BigDecimal)10 / (BigDecimal)3;
        result.ToString();
    }
    [Benchmark]
    public void DivideToString_BigFloat() {
        BigFloat result = (BigFloat)10 / (BigFloat)3;
        result.ToString();
    }

    [Benchmark]
    public void Parse_BigReal() {
        BigReal result = BigReal.Parse("12345.6789");
    }
    [Benchmark]
    public void Parse_BigDecimal() {
        BigDecimal result = BigDecimal.Parse("12345.6789");
    }
    [Benchmark]
    public void Parse_BigFloat() {
        BigFloat result = BigFloat.Parse("12345.6789");
    }

    [Benchmark]
    public void FromFloat_BigReal() {
        BigReal result = (BigReal)123.45f;
    }
    [Benchmark]
    public void FromFloat_BigDecimal() {
        BigDecimal result = (BigDecimal)123.45f;
    }
    [Benchmark]
    public void FromFloat_BigFloat() {
        BigFloat result = (BigFloat)123.45f;
    }
}

[MemoryDiagnoser]
public class FromFloatBenchmarks {
    [Benchmark]
    public BigReal ParseFromHalf() {
        return BigReal.Parse(((Half)23.4f).ToString(CultureInfo.InvariantCulture));
    }
    [Benchmark]
    public BigReal InterpretBitsFromHalf() {
        return (BigReal)(Half)23.4f;
    }
    [Benchmark]
    public BigReal ParseFromFloat() {
        return BigReal.Parse(23.4f.ToString(CultureInfo.InvariantCulture));
    }
    [Benchmark]
    public BigReal InterpretBitsFromFloat() {
        return (BigReal)23.4f;
    }
    [Benchmark]
    public BigReal ParseFromDouble() {
        return BigReal.Parse(23.4.ToString(CultureInfo.InvariantCulture));
    }
    [Benchmark]
    public BigReal InterpretBitsFromDouble() {
        return (BigReal)23.4;
    }
    [Benchmark]
    public BigReal ParseFromDecimal() {
        return BigReal.Parse(23.4m.ToString(CultureInfo.InvariantCulture));
    }
    [Benchmark]
    public BigReal InterpretBitsFromDecimal() {
        return (BigReal)23.4m;
    }
}