namespace NEG.CTF2.Core.Commands;

internal interface ICommand
{
	protected static readonly string escapeSequencePrefix = "\x1b";

	/// <returns>
	/// Returns the escape sequence, provided by Virtual Terminal Sequences (strictly on windows platforms as of now).
	/// </returns>
	/// <remarks>
	/// <see href="https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences">Virtual Terminal Sequencing</see>
	/// </remarks>
	public string GetUnderlyingConsoleValue();
}
