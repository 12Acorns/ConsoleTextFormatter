namespace NEG.CTF2.Core.Commands;

internal interface ICommand
{
	protected static readonly string escapeSequencePrefix = "\x1B[";

	/// <returns>
	/// The escape sequence used to call a Virtual Termianl Sequence command
	/// </returns>
	/// <remarks>
	/// <see href="https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences">Virtual Terminal Sequencing</see>
	/// </remarks>
	public string EscapeSequence { get; }
}
