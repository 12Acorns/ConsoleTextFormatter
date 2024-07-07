using NEG.CTF2.Core.Diagnostics;
using NEG.CTF2.Core.Tokens.Types;

namespace NEG.CTF2.Core.Tokens;

internal sealed class Lexer
{
	private const char COMMASEPERATOR = ',';
	private const char COLONSEPERATOR = ':';
	private const char SQUAREOPENBRACKET = '[';
	private const char SQUARECLOSEBRACKET = ']';

	public Lexer(string _text)
	{
		diagnosticResults = new();
		text = _text;
	}

	private readonly DiagnosticResults diagnosticResults;
	private readonly string text;

	private int position;

	public DiagnosticResults DiagnosticResults => diagnosticResults;

	private char currentChar
	{
		get
		{
			if(position >= text.Length)
			{
				return '\0';
			}
			return text[position];
		}
	}

	private (char _value, int _start) NextChar()
	{
		var _start = position;
		var _value = text[position];
		IncrementPosition();
		return (_value, _start);
	}
	private void IncrementPosition()
	{
		position++;
	}
	public IEnumerable<SyntaxToken> AllTokens()
	{
		position = 0;

		var _currentToken = NextToken();

		yield return _currentToken;

		while(_currentToken.Type is not TokenType.EndOfFile)
		{
			_currentToken = NextToken();

			if(_currentToken.Type is TokenType.Whitespace or TokenType.Invalid)
			{
				continue;
			}

			yield return _currentToken;
		}
	}
	public SyntaxToken NextToken()
	{
		if(position >= text.Length)
		{
			return new SyntaxToken(TokenType.EndOfFile, position, "\0", null);
		}

		if(WhitespaceToken() is SyntaxToken _whiteSpaceToken)
		{
			return _whiteSpaceToken;
		}

		if(NumericToken() is SyntaxToken _numericToken)
		{
			return _numericToken;
		}

		if(SingularToken() is SyntaxToken _singularToken)
		{
			return _singularToken;
		}

		var _diagnosticResult = new DiagnosticResult()
		{
			Type = TokenType.Invalid,
			Reason = "ERROR: Unexpected input",
			Position = position + 1,
			Value = string.Empty
		};

		DiagnosticResults.Add(_diagnosticResult);
		return new SyntaxToken(TokenType.Invalid, position++, string.Empty, null);
	}
	private SyntaxToken? WhitespaceToken()
	{
		if(!char.IsWhiteSpace(currentChar))
		{
			return null;
		}
		var _start = position;

		while(char.IsWhiteSpace(currentChar))
		{
			IncrementPosition();
		}
		var _length = position - _start;

		var _content = text.Substring(_start, _length);

		return new SyntaxToken(TokenType.Whitespace, _start, _content, null);
	}
	private SyntaxToken? NumericToken()
	{
		if(!char.IsDigit(currentChar))
		{
			return null;
		}
		var _start = position;

		bool _isByte = true;

		while(char.IsDigit(currentChar) || currentChar is '.')
		{
			if(currentChar is '.')
			{
				_isByte = false;
			}
			IncrementPosition();
		}

		var _length = position - _start;
		var _content = text.Substring(_start, _length);

		_isByte = byte.TryParse(_content, out var _byte);

		AddNumericDiagnostics(_isByte, _content);

		return new SyntaxToken(TokenType.Numeric, _start, _content, _byte);
	}
	private SyntaxToken? SingularToken()
	{
		if(ColonToken() is SyntaxToken _colonToken)
		{
			return _colonToken;
		}
		if(CommaToken() is SyntaxToken _commaToken)
		{
			return _commaToken;
		}
		if(SquareBracketToken() is SyntaxToken _squareBracketToken)
		{
			return _squareBracketToken;
		}
		if(OperatorToken() is SyntaxToken _operatorToken)
		{
			IncrementPosition();
			return _operatorToken;
		}
		return null;
	}
	private SyntaxToken? ColonToken()
	{
		if(currentChar is not COLONSEPERATOR)
		{
			return null;
		}
		var (_, _start) = NextChar();
		return new SyntaxToken(TokenType.Colon, _start, ":", ':');
	}
	private SyntaxToken? CommaToken()
	{
		if(currentChar is not COMMASEPERATOR)
		{
			return null;
		}
		var (_, _start) = NextChar();
		return new SyntaxToken(TokenType.Comma, _start, ",", ',');
	}
	private SyntaxToken? SquareBracketToken()
	{
		if(currentChar is not SQUAREOPENBRACKET and not SQUARECLOSEBRACKET)
		{
			return null;
		}
		var (_value, _start) = NextChar();

		if(_value is SQUAREOPENBRACKET)
		{
			return new SyntaxToken(TokenType.SquareOpenBracket, _start, "[", SQUAREOPENBRACKET);
		}
		return new SyntaxToken(TokenType.SquareCloseBracket, _start, "]", SQUARECLOSEBRACKET);
	}
	private SyntaxToken? OperatorToken() => currentChar switch
	{
		'+' => new SyntaxToken(TokenType.Addition, position, "+", null),
		'-' => new SyntaxToken(TokenType.Subtraction, position, "-", null),
		'*' => new SyntaxToken(TokenType.Multiplication, position, "*", null),
		'/' => new SyntaxToken(TokenType.Division, position, "/", null),

		_ => null
	};

	private void AddNumericDiagnostics(bool _isByte, string _content)
	{
		if(_isByte)
		{
			return;
		}

		var _result = new DiagnosticResult()
		{
			Type = TokenType.Invalid,
			Reason = "ERROR: Got type decimal, expected type byte",
			Position = position,
			Value = _content
		};
		diagnosticResults.Add(_result);
	}
}