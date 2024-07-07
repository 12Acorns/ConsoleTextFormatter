using NEG.CTF2.Core.Tokens.Nodes;

namespace NEG.CTF2.Core.Tokens.Types;

public sealed class SyntaxToken : SyntaxNode
{
	public SyntaxToken(TokenType _type, int _position, string _text, object? _value)
	{
		Type = _type;
		Position = _position;
		Text = _text;
		Value = _value;
	}

	public override TokenType Type { get; }
	public int Position { get; }
	public string Text { get; }
	public object? Value { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return [];
	}
}
