using Core.Tokens.Types;

namespace Core.Diagnostics;

internal readonly struct DiagnosticResult : IDiagnosticResult
{
	public DiagnosticResult(TokenType _type, string _reason)
	{
		Type = _type;
		Reason = _reason;
	}
	public TokenType Type { get; }
	public string Reason { get; }
}
