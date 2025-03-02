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

Trigonometry:
```cs
Console.WriteLine(BigReal.Sin(100).ToString(20)); // -0.50636564110975879365
```

## Performance

Basic operations comparing `BigReal`, [`BigDecimal`](https://github.com/AdamWhiteHat/BigDecimal), and [`BigFloat`](https://github.com/FaustVX/BigFloat):
- `Add`: `10 + 3`
- `Divide`: `10 / 3`
- `DivideToString`: `(10 / 3).ToString()`
- `Parse`: `Parse("12345.6789")`
- `FromFloat`: `123.45f`

| Method                    | Mean          | Error       | StdDev      | Gen0   | Allocated |
|-------------------------- |--------------:|------------:|------------:|-------:|----------:|
| Add_BigReal               |     21.691 ns |   0.0874 ns |   0.0818 ns |      - |         - |
| Add_BigDecimal            |    214.255 ns |   0.9779 ns |   0.9147 ns | 0.0153 |      48 B |
| Add_BigFloat              |     23.496 ns |   0.0683 ns |   0.0639 ns |      - |         - |
| Divide_BigReal            |      7.670 ns |   0.0221 ns |   0.0207 ns |      - |         - |
| Divide_BigDecimal         | 26,478.999 ns |  96.2829 ns |  80.4006 ns | 7.0190 |   22016 B |
| Divide_BigFloat           |     15.538 ns |   0.0551 ns |   0.0515 ns |      - |         - |
| DivideToString_BigReal    | 17,104.758 ns |  84.1202 ns |  78.6861 ns | 1.6785 |    5272 B |
| DivideToString_BigDecimal | 26,800.138 ns | 133.3113 ns | 118.1770 ns | 7.1716 |   22520 B |
| DivideToString_BigFloat   | 17,918.482 ns |  86.7425 ns |  81.1390 ns | 3.0212 |    9544 B |
| Parse_BigReal             |    295.278 ns |   1.3559 ns |   1.2683 ns | 0.0124 |      40 B |
| Parse_BigDecimal          |    211.807 ns |   1.1707 ns |   1.0951 ns | 0.0253 |      80 B |
| Parse_BigFloat            |    335.903 ns |   1.2944 ns |   1.1475 ns | 0.0124 |      40 B |
| FromFloat_BigReal         |    393.450 ns |   2.1953 ns |   1.9461 ns | 0.0229 |      72 B |
| FromFloat_BigDecimal      |    316.437 ns |   1.3594 ns |   1.2715 ns | 0.0329 |     104 B |
| FromFloat_BigFloat        |  1,779.366 ns |   4.8228 ns |   4.5113 ns | 0.2136 |     672 B |

Notes:
- Whereas `BigDecimal` divides by calculating the decimal digits at a certain precision, `BigReal` stores a fraction so the digits are only calculated at `ToString`.

## Background

`BigReal` is made by Joyless and based on [`BigFloat`](https://github.com/FaustVX/BigFloat) by FaustVX.
Some inspiration was taken from [`BigDecimal`](https://github.com/AdamWhiteHat/BigDecimal) by AdamWhiteHat.

## Gotchas

- The `default` value of `BigReal` is NaN, since 0 / 0 is NaN.
- Dividing by 0 gives positive or negative infinity depending on the numerator.