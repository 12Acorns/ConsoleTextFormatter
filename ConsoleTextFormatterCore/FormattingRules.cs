namespace NEG.CTF2.Core;

public sealed class FormattingRules
{
	/// <summary>
	/// Indicates if a command is defined on a line before the text to keep the line gap.
	/// <para></para>
	/// IE:
	/// <code>
	/// [FG: Red]
	/// AA
	/// </code>
	/// Will result in (if true):
	/// <code>
	/// ----
	/// AA
	/// </code>
	/// and (if false):
	/// <code>
	/// AA
	/// </code>
	/// NOTE: '-' represents whitespace
	/// </summary>
	public bool PreserveLineStructure { get; init; } = false;
	/// <summary>
	/// The sequence that indicates to a newline
	/// </summary>
	// "\r\n" is the default ending for string literals
	public string CustomLineEndSequence { get; init; } = "\r\n";
	internal int CharCutoff => PreserveLineStructure ? 0 : CustomLineEndSequence.Length;
}