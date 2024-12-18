using System.Text.RegularExpressions;
using NEG.CTF2.Core.Commands.Data;
using System.Text;

namespace NEG.CTF2.Core.Commands;

internal readonly struct FormatCommand : ICommand
{
	static FormatCommand()
	{
		var _formats = new Dictionary<string, int>()
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
		formatMapper = _formats.GetAlternateLookup<ReadOnlySpan<char>>();
	}

	private static readonly Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> formatMapper;

	public FormatCommand(scoped ReadOnlySpan<char> _format)
	{
		if(!formatMapper.TryGetValue(_format, out var _formatSequence))
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
		foreach(var _format in formatMapper.Dictionary)
		{
			if(_invalidFormat.CompareTo(_format.Key, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_builder.AppendLine($"-{_format.Key}");
			}
		}
		return _builder.ToString();
	}
}
