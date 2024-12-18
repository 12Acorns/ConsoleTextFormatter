using BenchmarkDotNet.Running;

namespace PerformanceTests;

internal sealed class Program
{
	public static void Main()
	{
		BenchmarkRunner.Run<FormattingPerformanceTests>();
	}
}
