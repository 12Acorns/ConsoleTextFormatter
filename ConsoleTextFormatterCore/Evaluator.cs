using NEG.CTF2.Core.Tokens.ExpressionTypes;
using NEG.CTF2.Core.Tokens.ExpressionTypes.Numeric;
using NEG.CTF2.Core.Tokens.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEG.CTF2.Core;

public class Evaluator
{
	public Evaluator(ExpressionSyntax _root)
	{
		root = _root;
	}

	internal readonly ExpressionSyntax root;

	public int Evaluate()
	{
		return EvaluateExpression(root);
	}

	internal int EvaluateExpression(ExpressionSyntax _node)
	{
		switch(_node)
		{
			case NumericSyntaxExpression _numericSyntax:
			{
				return Convert.ToInt32(_numericSyntax.Token.Value!);
			}
			case BinaryExpression _binaryExpression:
			{
				var _left = EvaluateExpression(_binaryExpression.Left);
				var _right = EvaluateExpression(_binaryExpression.Right);

				return _binaryExpression.Operator.Type switch
				{
					TokenType.Addition => _left + _right,
					TokenType.Subtraction => _left - _right,
					TokenType.Multiplication => _left * _right,
					TokenType.Division => _left / _right,

					_ => throw new Exception($"Unexpected binary operation: {_binaryExpression.Operator.Type}")
				};
			}
			default:
			{
				throw new Exception($"Unexpected node: {_node.Type}");
			}
		}
	}
}
