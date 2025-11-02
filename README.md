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
Console.WriteLine(BigReal.Pow(3.4, 2)); // 11.559999999999999
```

Parse from string:
```cs
Console.WriteLine(BigReal.Parse("12.34")); // 12.34
Console.WriteLine(BigReal.Parse("2e2.5").ToString(2)); // 632.45
```

Trigonometry:
```cs
Console.WriteLine(BigReal.Sin(100).ToString(20)); // -0.5063656411097587906
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
| Add_BigReal               |     20.342 ns |   0.0616 ns |   0.0514 ns |      - |         - |
| Add_BigDecimal            |    373.701 ns |   2.4290 ns |   2.0284 ns | 0.0739 |     464 B |
| Add_BigFloat              |     21.680 ns |   0.0568 ns |   0.0504 ns |      - |         - |
| Divide_BigReal            |      9.109 ns |   0.0228 ns |   0.0190 ns |      - |         - |
| Divide_BigDecimal         | 28,783.122 ns | 134.7718 ns | 112.5406 ns | 5.2185 |   32832 B |
| Divide_BigFloat           |     14.687 ns |   0.3137 ns |   0.3356 ns |      - |         - |
| DivideToString_BigReal    |  1,165.798 ns |   3.4006 ns |   3.1809 ns | 0.0534 |     344 B |
| DivideToString_BigDecimal |  2,822.685 ns |  24.7819 ns |  19.3481 ns | 0.5226 |    3296 B |
| DivideToString_BigFloat   |  1,385.152 ns |  19.1418 ns |  16.9687 ns | 0.1869 |    1176 B |
| Parse_BigReal             |    272.192 ns |   1.9640 ns |   1.6400 ns | 0.0062 |      40 B |
| Parse_BigDecimal          |    243.745 ns |   1.3348 ns |   1.0421 ns | 0.0291 |     184 B |
| Parse_BigFloat            |    311.843 ns |   5.5915 ns |   4.9568 ns | 0.0062 |      40 B |
| FromFloat_BigReal         |    110.971 ns |   0.3906 ns |   0.3262 ns |      - |         - |
| FromFloat_BigDecimal      |    322.433 ns |   0.9244 ns |   0.7719 ns | 0.0329 |     208 B |
| FromFloat_BigFloat        |  1,609.420 ns |   5.7215 ns |   4.7777 ns | 0.1068 |     672 B |

Notes:
- Whereas `BigDecimal` divides by calculating the decimal digits at a certain precision, `BigReal` stores a fraction so the digits are only calculated at `ToString`.

## Background

`BigReal` is made by Joyless and based on [`BigFloat`](https://github.com/FaustVX/BigFloat) by FaustVX.
Some inspiration was taken from [`BigDecimal`](https://github.com/AdamWhiteHat/BigDecimal) by AdamWhiteHat.

## Gotchas

- The `default` value of `BigReal` is NaN, since 0 / 0 is NaN.
- Dividing by 0 gives positive or negative infinity depending on the numerator.