using NEG.CTF2.Core;

namespace NEG.CTF2.Core.ExampleUsage;

internal sealed class Program
{
	private const string BASICTEXTTEST =
		"""
		[FG: Red, Bold]
		Blah blah

		[BG: Blue]
		SSS
		""";
	static void Main()
	{
		var _text = QuickFormat.Format(BASICTEXTTEST, new FormattingRules()
		{
			PreserveLineStructure = false,
		});

		Console.WriteLine($"{QuickFormat.Format("[Bold, UnderLine]Original:")}\n{BASICTEXTTEST}\n");
		Console.WriteLine($"{QuickFormat.Format("[Bold, UnderLine]Formatted:")}\n{_text}\n");

		Console.WriteLine(
			"""
			Try writing your own formatted text!
			For more info type 'format -h'
			To do a format, type 'format "[TEXT HERE]"'

			""");

		var _ownFormat = Console.ReadLine();

		Console.WriteLine();
		Console.WriteLine(QuickFormat.Format(_ownFormat!));
	}
}
