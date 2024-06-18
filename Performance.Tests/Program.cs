using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Performance.Tests;
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args);
        }
    }
public class OurBenchmarks 
{
    [Benchmark]
    public void OurBenchmark() 
    { }
}