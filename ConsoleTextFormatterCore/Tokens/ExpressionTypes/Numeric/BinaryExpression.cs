using NEG.CTF2.Core.Tokens.Nodes;
using NEG.CTF2.Core.Tokens.Types;

namespace NEG.CTF2.Core.Tokens.ExpressionTypes.Numeric;

// For numeric operations, like + - * / % ect
public sealed class BinaryExpression : ExpressionSyntax
{
	public BinaryExpression(ExpressionSyntax _left, SyntaxToken _operator, ExpressionSyntax _right)
	{
		Left = _left;
		Operator = _operator;
		Right = _right;
	}

	public override TokenType Type => TokenType.BinaryExpression;
	public ExpressionSyntax Left { get; }
	public SyntaxToken Operator { get; }
	public ExpressionSyntax Right { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return Operator;
		yield return Right;
	}
}
