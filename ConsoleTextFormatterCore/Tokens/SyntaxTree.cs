using NEG.CTF2.Core.Diagnostics;
using NEG.CTF2.Core.Tokens.ExpressionTypes;
using NEG.CTF2.Core.Tokens.Types;

namespace NEG.CTF2.Core.Tokens;

public sealed class SyntaxTree
{
	public SyntaxTree(ExpressionSyntax _root, SyntaxToken _endOfFile, DiagnosticResults _results,
		IEnumerable<SyntaxToken> _tokens)
	{
		EndOfFile = _endOfFile;
		DiagnosticResults = _results;
		Root = _root;
		tokens = _tokens;
	}

	public DiagnosticResults DiagnosticResults { get; }
	public ExpressionSyntax Root { get; }
	public SyntaxToken EndOfFile { get; }
	public IEnumerable<SyntaxToken> tokens { get; }

	public static SyntaxTree Parse(string _input)
	{
		var _lexer = new Lexer(_input);

		var _tokens = _lexer.AllTokens();

		var _parser = new Parser(_lexer.DiagnosticResults, _tokens);

		return _parser.Parse();
	}
}
