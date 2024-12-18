using NEG.CTF2.Core.Extensions;

namespace NEG.CTF2.Core.Commands;

internal interface ICommand
{
	private const string ESCAPESEQUENCEPREFIX = "\x1B[";
	private const string ESCAPESEQUENCEEND = "m";

	private static readonly int entireEscapeSequenceLength = ESCAPESEQUENCEEND.Length + ESCAPESEQUENCEPREFIX.Length;

	/// <returns>
	/// The escape sequence used to call a Virtual Termianl Sequence command
	/// </returns>
	/// <remarks>
	/// <see href="https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences">Virtual Terminal Sequencing</see>
	/// </remarks>
	public string EscapeSequence { get; }

	protected static string Format(string _sequence) =>
		string.Format(ESCAPESEQUENCEPREFIX, _sequence);
	protected static string Format(int _sequence)
	{
		var _intLength = _sequence.IntLength();
		var _wholeLength = ESCAPESEQUENCEPREFIX.Length + 1 + _intLength;
		Span<char> _wholeBuffer = stackalloc char[_wholeLength];
		int i;
		for(i = 0; i < ESCAPESEQUENCEPREFIX.Length; i++)
		{
			_wholeBuffer[i] = ESCAPESEQUENCEPREFIX[i];
		}
		var _tmp = _sequence;
		for(int j = _intLength - 1; j >= 0; j--)
		{
			var _a = _tmp / 10;
			var _b = _tmp - _a * 10;
			_wholeBuffer[i + j] = (char)(_b + '0');
			_tmp = _a;
		}
		_wholeBuffer[^1] = 'm';

		return _wholeBuffer.ToString();
	}
}
