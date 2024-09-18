using NEG.CTF2.Core;

namespace NEG.CTF2.Core.ExampleUsage;

internal sealed class Program
{
	private const string BASICTEXTTEST =
		"""
		[FG: Red, BG: Blue, Underline, Bold]
		Blah blah
		""";
	static void Main()
	{
		var _formatter = new TextFormatter(BASICTEXTTEST);
		_formatter.GenerateFormat();

		Console.ReadKey();
	}
}
