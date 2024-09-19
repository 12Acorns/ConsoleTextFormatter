using NEG.CTF2.Core;

namespace NEG.CTF2.Core.ExampleUsage;

internal sealed class Program
{
	private const string BASICTEXTTEST =
		"""
		[FG: Red, Bold, No UnderLine]
		Blah blah
		""";
	static void Main()
	{
		var _rules = new FormattingRules()
		{
			PreserveLineStructure = false,
		};
		var _formatter = new TextFormatter(BASICTEXTTEST, _rules);
		var _text = _formatter.GenerateFormat();

		Console.WriteLine(_text);

		Console.ReadKey();
	}
}
