using NEG.CTF2.Core.Commands.Data;
using System.Text.RegularExpressions;

namespace NEG.CTF2.Core.Commands;

internal sealed class FormatCommand : ICommand
{
	private static readonly Dictionary<string, int> formatMapper = new()
	{
		// Colour but here since requires more refactoring to do so
		// and i doubt most would argue that colour could be in here :/
		{ "Invert", 7 },
		{ "No Invert", 27 },

		// Format
		{ "Bold", 1 },
		{ "UnderLine", 4 },

		// Unformat
		{ "No Bold", 22 },
		{ "No UnderLine", 24 },
	};

	public FormatCommand(scoped ReadOnlySpan<char> _format)
	{
		if(!formatMapper.TryGetValue(_format.ToString(), out var _formatSequence))
		{
			throw new Exception($"Inputted format '{_format}', is not a valid format. " +
				"Please enter a valid format.");
		}
		EscapeSequence = ICommand.Format(_formatSequence);
	}
	public string EscapeSequence { get; }
}
