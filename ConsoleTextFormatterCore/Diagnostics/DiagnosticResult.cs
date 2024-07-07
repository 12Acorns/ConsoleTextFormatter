using System.Diagnostics.CodeAnalysis;
using NEG.CTF2.Core.Tokens.Types;

namespace NEG.CTF2.Core.Diagnostics;

public readonly struct DiagnosticResult : IDiagnosticResult
{
	[SetsRequiredMembers]
	public DiagnosticResult(TokenType _type, string _reason, string _value, int _position)
	{
		Type = _type;
		Value = _value;
		Reason = _reason;
		Position = _position;
	}
	public required TokenType Type { get; init; }
	public required string Reason { get; init; }
	public required string Value { get; init; }
	public required int Position { get; init; }

	public override string ToString()
	{
		return
			$"{{\n    Type: {Type}\n    Reason: {Reason}\n    Value: {Value}\n}}\n";
	}
}
