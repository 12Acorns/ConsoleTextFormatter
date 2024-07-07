using NEG.CTF2.Core.Tokens;
using NEG.CTF2.Core.Tokens.ExpressionTypes;
using NEG.CTF2.Core.Tokens.Nodes;
using NEG.CTF2.Core.Tokens.Types;
using System.Text;

namespace NEG.CTF2.Core.Visualization;

/// <summary>
/// Returns a user friendly string as a output for the user class to print to console or elsewhere
/// </summary>
public sealed class SyntaxTreeVisualizer
{
	private const string BRANCHANDCONTINUE = "├──";
	private const string CONTINUENOBRANCH = "│  ";
	private const string ENDBRANCH = "└──";

	public SyntaxTreeVisualizer(SyntaxTree _tree)
	{
		tree = _tree;
	}

	private readonly SyntaxTree tree;

	public string ShowErrors()
	{
		return tree.DiagnosticResults.ToString();
	}
	public string ShowTree()
	{
		return PrettyPrint(tree.Root);
	}
	public string ShowTokens()
	{
		return WriteTokens();
	}

	private static string PrettyPrint(
		SyntaxNode _node,
		string _indent = "",
		bool _isLast = true)
	{

		var _treeMarker = _isLast ? ENDBRANCH : BRANCHANDCONTINUE;

		var _builder = new StringBuilder(_indent + _treeMarker + _node.Type);

		if(_node is SyntaxToken _token && _token.Value != null)
		{
			_builder.Append(" " + _token.Value);
		}

		_builder.AppendLine();

		_indent += _isLast ? "   " : CONTINUENOBRANCH;

		var _children = _node.GetChildren();

		var _lastChild = _children.LastOrDefault();

		foreach(var _tokenChild in _children)
		{
			_builder.Append(PrettyPrint(_tokenChild, _indent, _tokenChild == _lastChild));
		}

		return _builder.ToString();
	}
	private string WriteTokens()
	{
		var _builder = new StringBuilder();

		foreach(var _token in tree.tokens)
		{
			if(_token.Type is TokenType.EndOfFile)
			{
				continue;
			}

			_builder.AppendLine(WriteToken(_token));
		}
		return _builder.ToString();
	}
	private static string WriteToken(SyntaxToken _currentToken)
	{
		return $"{_currentToken.Type} | {_currentToken.Text} | " +
			_currentToken.Value ?? "Null";
	}
}
