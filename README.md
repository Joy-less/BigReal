# BigReal

[![NuGet](https://img.shields.io/nuget/v/BigReal.svg)](https://www.nuget.org/packages/BigReal)

An arbitrary size and precision rational number stored using two BigIntegers.

## Usage

Create from fixed-size numbers:
```cs
Console.WriteLine(new BigReal(100)); // 100
Console.WriteLine((BigReal)3.14); // 3.14
Console.WriteLine(new BigReal(4, 3).ToString(4)); // 1.3333
```

Perform basic operations:
```cs
Console.WriteLine((BigReal)3 * (BigReal)5); // 15
Console.WriteLine(BigReal.Pow(3.4, 2)); // 11.56
```

Parse from string:
```cs
Console.WriteLine(BigReal.Parse("12.34")); // 12.34
Console.WriteLine(BigReal.Parse("2e2.5").ToString(2)); // 632.45
```

## Performance

Basic operations comparing `BigReal`, [`BigDecimal`](https://github.com/AdamWhiteHat/BigDecimal), and [`BigFloat`](https://github.com/FaustVX/BigFloat):
- `Add`: `10 + 3`
- `Divide`: `10 / 3`
- `DivideToString`: `(10 / 3).ToString()`

| Method                    | Mean          | Error       | StdDev      | Gen0   | Allocated |
|-------------------------- |--------------:|------------:|------------:|-------:|----------:|
| Add_BigReal               |     22.792 ns |   0.0487 ns |   0.0432 ns |      - |         - |
| Add_BigDecimal            |    216.776 ns |   0.8178 ns |   0.7650 ns | 0.0153 |      48 B |
| Add_BigFloat              |     23.473 ns |   0.0593 ns |   0.0554 ns |      - |         - |
| Divide_BigReal            |      7.759 ns |   0.0333 ns |   0.0296 ns |      - |         - |
| Divide_BigDecimal         | 27,114.541 ns | 248.3895 ns | 232.3437 ns | 7.0190 |   22016 B |
| Divide_BigFloat           |     15.682 ns |   0.0475 ns |   0.0445 ns |      - |         - |
| DivideToString_BigReal    | 17,448.714 ns |  78.7370 ns |  73.6507 ns | 1.6785 |    5272 B |
| DivideToString_BigDecimal | 27,533.222 ns | 128.9970 ns | 114.3525 ns | 7.1716 |   22520 B |
| DivideToString_BigFloat   | 18,121.263 ns |  65.5024 ns |  61.2710 ns | 3.0212 |    9544 B |

Notes:
- Whereas `BigDecimal` divides by calculating the decimal digits at a certain precision, `BigReal` stores a fraction so the digits are only calculated at `ToString`.

## Background

`BigReal` is based on [`BigFloat`](https://github.com/FaustVX/BigFloat) by FaustVX.
Significant changes were made by Joyless.