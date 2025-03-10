using ExtendedNumerics;

Console.WriteLine(BigReal.CalculatePi(2));
Console.WriteLine(BigReal.CalculatePi(10));
Console.WriteLine(BigReal.CalculatePi(1000).ToString(1000));

Console.WriteLine(new BigReal(-50, -50));
Console.WriteLine(new BigReal(-50, -50) < (1000000000));
Console.WriteLine((new BigReal(-50) / -50) < (1000000000));
Console.WriteLine(new BigReal(1) < (1000000000));

Console.WriteLine(new BigReal(100)); // 100
Console.WriteLine((BigReal)3.14); // 3.14
Console.WriteLine(new BigReal(4, 3).ToString(4)); // 1.3333

Console.WriteLine((BigReal)3 * (BigReal)5); // 15
Console.WriteLine(BigReal.Pow(3.4, 2)); // 11.56

Console.WriteLine(BigReal.Parse("12.34")); // 12.34
Console.WriteLine(BigReal.Parse("2e2.5").ToString(2)); // 632.45

Console.WriteLine(BigReal.Truncate(BigReal.Sin(100), 20)); // -0.50636564110975879365
Console.WriteLine(BigReal.Truncate(BigReal.Cos(100), 20)); // 0.8623188722876839341
Console.WriteLine(BigReal.Truncate(BigReal.Tan(100), 20)); // -0.58721391515692907667

Console.WriteLine(BigReal.Truncate(BigReal.Sec(100), 20)); // 1.15966382290469383255
Console.WriteLine(BigReal.Truncate(BigReal.Cosec(100), 20)); // -1.97485753142409996121
Console.WriteLine(BigReal.Truncate(BigReal.Cot(100), 20)); // -1.7029569194264692161
return;

Console.WriteLine("===========================");

Console.WriteLine(new BigReal(0.012345).ToString(3));
Console.WriteLine(new BigReal(1234500).ToString(3));
Console.WriteLine(BigReal.Truncate(new BigReal(1234500.678)).ToString(3));
Console.WriteLine(BigReal.LeftShift(new BigReal(1234500.678), numberBase: 10).ToString(3));
Console.WriteLine(default(BigReal) * 3);

Console.WriteLine(BigReal.Parse("nan"));
Console.WriteLine(new BigReal(23.4f));
Console.WriteLine(double.NegativeInfinity);
Console.WriteLine(double.Abs(double.NegativeInfinity));

Console.WriteLine(BigReal.Pow(10, -2));
Console.WriteLine(Math.Pow(10, -2));

Console.WriteLine(BigReal.Pow(4, -2));
Console.WriteLine(BigReal.Pow(-4, 2));
Console.WriteLine(BigReal.Sqrt(4));
Console.WriteLine(BigReal.Root(4, -1));

Console.WriteLine(BigReal.Log(5));
Console.WriteLine(BigReal.Log10(5));
Console.WriteLine(BigReal.Log(100, 2));
Console.WriteLine(BigReal.Log2(100));

Console.WriteLine(BigReal.Pow(4.5, 4.5));

Console.WriteLine(BigReal.LeftShift(new BigReal(10)));
Console.WriteLine(BigReal.RightShift(new BigReal(10)));