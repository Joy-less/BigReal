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
    public BigReal Add_BigReal() {
        return (BigReal)10 + (BigReal)3;
    }
    [Benchmark]
    public BigDecimal Add_BigDecimal() {
        return (BigDecimal)10 + (BigDecimal)3;
    }
    [Benchmark]
    public BigFloat Add_BigFloat() {
        return (BigFloat)10 + (BigFloat)3;
    }

    [Benchmark]
    public BigReal Divide_BigReal() {
        return (BigReal)10 / (BigReal)3;
    }
    [Benchmark]
    public BigDecimal Divide_BigDecimal() {
        return (BigDecimal)10 / (BigDecimal)3;
    }
    [Benchmark]
    public BigFloat Divide_BigFloat() {
        return (BigFloat)10 / (BigFloat)3;
    }

    [Benchmark]
    public string DivideToString_BigReal() {
        BigReal result = (BigReal)10 / (BigReal)3;
        return result.ToString();
    }
    [Benchmark]
    public string DivideToString_BigDecimal() {
        BigDecimal result = (BigDecimal)10 / (BigDecimal)3;
        return result.ToString();
    }
    [Benchmark]
    public string DivideToString_BigFloat() {
        BigFloat result = (BigFloat)10 / (BigFloat)3;
        return result.ToString();
    }

    [Benchmark]
    public BigReal Parse_BigReal() {
        return BigReal.Parse("12345.6789");
    }
    [Benchmark]
    public BigDecimal Parse_BigDecimal() {
        return BigDecimal.Parse("12345.6789");
    }
    [Benchmark]
    public BigFloat Parse_BigFloat() {
        return BigFloat.Parse("12345.6789");
    }

    [Benchmark]
    public BigReal FromFloat_BigReal() {
        return (BigReal)123.45f;
    }
    [Benchmark]
    public BigDecimal FromFloat_BigDecimal() {
        return (BigDecimal)123.45f;
    }
    [Benchmark]
    public BigFloat FromFloat_BigFloat() {
        return (BigFloat)123.45f;
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