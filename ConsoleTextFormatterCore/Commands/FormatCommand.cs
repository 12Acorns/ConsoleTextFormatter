using System.Text;
using System.Text.RegularExpressions;
using NEG.CTF2.Core.Commands.Data;

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
				$"Perhaps you meant?\n{ShowBySimilarity(_format)}");
		}
		EscapeSequence = ICommand.Format(_formatSequence);
	}
	public string EscapeSequence { get; }

	private static string ShowBySimilarity(scoped ReadOnlySpan<char> _invalidFormat)
	{
		var _builder = new StringBuilder();
		foreach(var _format in formatMapper)
		{
			if(_invalidFormat.CompareTo(_format.Key, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_builder.AppendLine($"-{_format.Key}");
			}
		}
		return _builder.ToString();
	}
}
