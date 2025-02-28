using BigFloatSharp;

Console.WriteLine(new BigFloat(0.012345).ToString(3));
Console.WriteLine(new BigFloat(1234500).ToString(3));
Console.WriteLine(BigFloat.Truncate(new BigFloat(1234500.678)).ToString(3));