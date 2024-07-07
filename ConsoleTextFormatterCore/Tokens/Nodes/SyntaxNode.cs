using NEG.CTF2.Core.Tokens.Types;

namespace NEG.CTF2.Core.Tokens.Nodes;

public abstract class SyntaxNode
{
	public abstract TokenType Type { get; }

	public abstract IEnumerable<SyntaxNode> GetChildren();
}
