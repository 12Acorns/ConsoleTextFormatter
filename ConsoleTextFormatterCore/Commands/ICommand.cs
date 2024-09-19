namespace NEG.CTF2.Core.Commands;

internal interface ICommand
{
	private static readonly string escapeSequencePrefix = "\x1B[{0}m";

	/// <returns>
	/// The escape sequence used to call a Virtual Termianl Sequence command
	/// </returns>
	/// <remarks>
	/// <see href="https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences">Virtual Terminal Sequencing</see>
	/// </remarks>
	public string EscapeSequence { get; }

	protected static string Format(string _sequence) =>
		string.Format(escapeSequencePrefix, _sequence);
	protected static string Format(int _sequence) =>
	string.Format(escapeSequencePrefix, _sequence);
}
