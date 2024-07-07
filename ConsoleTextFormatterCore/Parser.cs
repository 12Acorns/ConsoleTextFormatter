using NEG.CTF2.Core.Diagnostics;
using NEG.CTF2.Core.Tokens;
using NEG.CTF2.Core.Tokens.ExpressionTypes;
using NEG.CTF2.Core.Tokens.ExpressionTypes.Numeric;
using NEG.CTF2.Core.Tokens.Types;
using System.Text;

namespace NEG.CTF2.Core;

// Not a binary expression
//            [
//           / \
//          FG  ,
//          |  / \
//          : BG  \
//          |   \  \
//          Red  :  \
//               |   \
//              Blue |\
//                   | \
//                   |  \
//                   |   \
//              Underline \
//                         ,
//                         |
//                        Bold
//
public sealed class Parser
{
	internal Parser(DiagnosticResults _results, IEnumerable<SyntaxToken> _tokens)
	{
		tokens = _tokens.ToArray();

		diagnosticResults = _results;
	}

	internal DiagnosticResults diagnosticResults;
	internal readonly IReadOnlyList<SyntaxToken> tokens;

	private int position;

	private SyntaxToken currentToken => PeekAhead(0);

	public SyntaxTree Parse()
	{
		var _expression = ParseTerm();
		var _endOfFileToken = Match(TokenType.EndOfFile);
		return new SyntaxTree(_expression, _endOfFileToken, diagnosticResults, tokens);
	}

	private ExpressionSyntax ParseTerm()
	{
		var _left = ParseFactor();

		while(
			   currentToken.Type is TokenType.Addition
			|| currentToken.Type is TokenType.Subtraction)
		{
			var _operator = NextToken();
			var _right = ParseFactor();
			_left = new BinaryExpression(_left, _operator, _right);
		}

		return _left;
	}
	private ExpressionSyntax ParseFactor()
	{
		var _left = ParsePrimaryBinaryExpression();

		while(
			   currentToken.Type is TokenType.Multiplication
			|| currentToken.Type is TokenType.Division)
		{
			var _operator = NextToken();
			var _right = ParsePrimaryBinaryExpression();
			_left = new BinaryExpression(_left, _operator, _right);
		}

		return _left;
	}
	private ExpressionSyntax ParsePrimaryBinaryExpression()
	{
		var _numericToken = Match(TokenType.Numeric);

		return new NumericSyntaxExpression(_numericToken);
	}
	private SyntaxToken Match(TokenType _expected)
	{
		var _currentToken = currentToken;
		var _position = position;
		if(currentToken.Type == _expected)
		{
			return NextToken();
		}

		var _diagnostics = new DiagnosticResult()
		{
			Type = _expected,
			Reason = $"Got <{_currentToken}>, expected <{_expected}>",
			Position = _position,
			Value = ""
		};
		diagnosticResults.Add(_diagnostics);
		return new SyntaxToken(_expected, currentToken.Position, "", null);
	}
	private SyntaxToken NextToken()
	{
		var _current = currentToken;
		position++;
		return _current;
	}
	private SyntaxToken PeekAhead(int _offset)
	{
		var _index = position + _offset;
		if(_index >= tokens.Count)
		{
			return tokens[^1];
		}
		return tokens[_index];
	}
}
