using BigFloatSharp;
using System.Numerics;

Console.WriteLine(new BigFloat(0.012345).ToString(3));
Console.WriteLine(new BigFloat(1234500).ToString(3));
Console.WriteLine(BigFloat.Truncate(new BigFloat(1234500.678)).ToString(3));
Console.WriteLine(BigFloat.ShiftLeft(new BigFloat(1234500.678), numberBase: 10).ToString(3));
//Console.WriteLine(default(BigFloat) * 3);

Console.WriteLine(BigFloat.Parse("nan"));
Console.WriteLine(new BigFloat(23.4f));