using Core.Tokens.Types;

namespace Core.Diagnostics;

internal interface IDiagnosticResult
{
	public TokenType Type { get; }
	public string Reason { get; }
}
