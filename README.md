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
| Add_BigReal               |     20.663 ns |   0.2319 ns |   0.2056 ns |      - |         - |
| Add_BigDecimal            |    196.293 ns |   0.7841 ns |   0.6951 ns | 0.0076 |      48 B |
| Add_BigFloat              |     21.373 ns |   0.1374 ns |   0.1148 ns |      - |         - |
| Divide_BigReal            |      7.021 ns |   0.0463 ns |   0.0433 ns |      - |         - |
| Divide_BigDecimal         | 23,632.775 ns |  76.0503 ns |  63.5055 ns | 3.5095 |   22016 B |
| Divide_BigFloat           |     14.317 ns |   0.0283 ns |   0.0251 ns |      - |         - |
| DivideToString_BigReal    |  1,190.507 ns |   4.9752 ns |   4.4103 ns | 0.0629 |     400 B |
| DivideToString_BigDecimal | 24,023.287 ns | 181.1982 ns | 169.4929 ns | 3.5706 |   22520 B |
| DivideToString_BigFloat   | 16,281.624 ns |  72.3191 ns |  67.6473 ns | 1.4954 |    9544 B |
| Parse_BigReal             |    283.959 ns |   1.5225 ns |   1.3497 ns | 0.0062 |      40 B |
| Parse_BigDecimal          |    185.566 ns |   0.5260 ns |   0.4392 ns | 0.0126 |      80 B |
| Parse_BigFloat            |    304.154 ns |   0.9946 ns |   0.7765 ns | 0.0062 |      40 B |
| FromFloat_BigReal         |    106.022 ns |   0.5105 ns |   0.4775 ns |      - |         - |
| FromFloat_BigDecimal      |    275.546 ns |   0.7714 ns |   0.6441 ns | 0.0162 |     104 B |
| FromFloat_BigFloat        |  1,663.926 ns |   8.3387 ns |   7.8000 ns | 0.1068 |     672 B |

Notes:
- Whereas `BigDecimal` divides by calculating the decimal digits at a certain precision, `BigReal` stores a fraction so the digits are only calculated at `ToString`.

## Background

`BigReal` is made by Joyless and based on [`BigFloat`](https://github.com/FaustVX/BigFloat) by FaustVX.
Some inspiration was taken from [`BigDecimal`](https://github.com/AdamWhiteHat/BigDecimal) by AdamWhiteHat.

## Gotchas

- The `default` value of `BigReal` is NaN, since 0 / 0 is NaN.
- Dividing by 0 gives positive or negative infinity depending on the numerator.