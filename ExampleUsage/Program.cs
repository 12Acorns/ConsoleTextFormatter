using NEG.CTF2.Core;
using NEG.CTF2.Core.Diagnostics;
using NEG.CTF2.Core.Tokens;
using NEG.CTF2.Core.Visualization;

namespace NEG.CTF2.Core.ExampleUsage;

internal class Program
{
	private const string INVALIDINPUTTEST = "1.1 + 2.2";
	private const string LEXERTOKENTEST = "[, :   09]";
	private const string BINARYTREETEST = "1 + 2 * 3";
	private const string VALIDLEXERTOKENTEST =
		"""
		[FG: Red, BG: Blue, Underline, Bold]
		Blah blah
		""";
	static void Main()
	{
		Console.WriteLine("---------------------------");
		Console.WriteLine("Type   |   Text   |   Value");

		var _tree = SyntaxTree.Parse(BINARYTREETEST);
		var _visualizer = new SyntaxTreeVisualizer(_tree);

		Console.WriteLine(_visualizer.ShowTokens());

		Console.WriteLine("---------------------------");

		Console.WriteLine("\n\n" + _visualizer.ShowTree());

		Console.WriteLine(_visualizer.ShowErrors());

		if(!_tree.DiagnosticResults.HasError)
		{
			var _evalutor = new Evaluator(_tree.Root);

			Console.WriteLine(_evalutor.Evaluate());
		}

		Console.ReadKey();
	}
}
