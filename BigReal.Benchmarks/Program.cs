using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ExtendedNumerics;

namespace ExtendedNumerics.Benchmarks;

public class Program {
    public static void Main() {
        BenchmarkSwitcher.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()).Run();
    }
}

[MemoryDiagnoser]
public class Benchmarks {
    /*[Benchmark]
    public void Add_BigFloat() {
        BigFloat result = (BigFloat)10 + (BigFloat)3;
    }
    [Benchmark]
    public void Add_BigDecimal() {
        BigDecimal result = (BigDecimal)10 + (BigDecimal)3;
    }
    [Benchmark]
    public void Add_BigFloatFaustVX() {
        System.Numerics.BigFloat result = (System.Numerics.BigFloat)10 + (System.Numerics.BigFloat)3;
    }

    [Benchmark]
    public void Divide_BigFloat() {
        BigFloat result = (BigFloat)10 / (BigFloat)3;
    }
    [Benchmark]
    public void Divide_BigDecimal() {
        BigDecimal result = (BigDecimal)10 / (BigDecimal)3;
    }
    [Benchmark]
    public void Divide_BigFloatFaustVX() {
        System.Numerics.BigFloat result = (System.Numerics.BigFloat)10 / (System.Numerics.BigFloat)3;
    }*/

    [Benchmark]
    public void DivideToString_BigFloat() {
        BigReal result = (BigReal)10 / (BigReal)3;
        result.ToString();
    }
    [Benchmark]
    public void DivideToString_BigDecimal() {
        BigDecimal result = (BigDecimal)10 / (BigDecimal)3;
        result.ToString();
    }
    [Benchmark]
    public void DivideToString_BigFloatFaustVX() {
        System.Numerics.BigFloat result = (System.Numerics.BigFloat)10 / (System.Numerics.BigFloat)3;
        result.ToString();
    }
}