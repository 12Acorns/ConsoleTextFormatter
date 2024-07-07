using NEG.CTF2.Core.Tokens.Types;

namespace NEG.CTF2.Core.Tokens.ExpressionTypes.Numeric;

public sealed class NumericSyntaxExpression : ExpressionSyntax
{
	public NumericSyntaxExpression(SyntaxToken _token)
	{
		Token = _token;
	}

	public override TokenType Type => TokenType.NumericExpression;
	public SyntaxToken Token { get; }

	public override IEnumerable<SyntaxToken> GetChildren()
	{
		yield return Token;
	}
}
