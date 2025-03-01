using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Numerics;

namespace ExtendedNumerics.Benchmarks;

public class Program {
    public static void Main() {
        BenchmarkSwitcher.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()).Run();
    }
}

[MemoryDiagnoser]
public class Benchmarks {
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
}