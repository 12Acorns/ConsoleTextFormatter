using NEG.CTF2.Core.Tokens.Types;

namespace NEG.CTF2.Core.Diagnostics;

internal interface IDiagnosticResult
{
	public TokenType Type { get; init; }
	public string Reason { get; init; }
	public string Value { get; init; }
	public int Position { get; init; }
}
