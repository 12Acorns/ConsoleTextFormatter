namespace NEG.CTF2.Core.Tokens.Types;

public enum TokenType
{
	// Tokens
	Whitespace,
	Colon,
	Comma,
	SquareOpenBracket,
	SquareCloseBracket,

	Numeric,
	Literal,

	EndOfFile,

	Invalid,

	// Binary Operations (Tmp)
	Addition,
	Subtraction,
	Multiplication,
	Division,

	// Expressions
	BinaryExpression,
	LiteralExpression,
	NumericExpression,
}
