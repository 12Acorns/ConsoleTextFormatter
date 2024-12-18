using BenchmarkDotNet.Attributes;
using NEG.CTF2.Core;

namespace PerformanceTests;

[MemoryDiagnoser(true)]
public class FormattingPerformanceTests
{
	public static readonly string testString =
		"""
		[FG: Red, Bold]
		Yes i am a string

		[No Bold]
		Look i am clean
		""";
	private FormattingRules? rules;

	[GlobalSetup]
	public void Setup()
	{
		rules = new FormattingRules();
	}

	[Benchmark]
	public string FormatPTestOne()
	{
		return new TextFormatter(testString, rules!).GenerateFormat();
	}
}
