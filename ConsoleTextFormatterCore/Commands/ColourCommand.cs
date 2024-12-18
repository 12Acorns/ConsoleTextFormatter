﻿using NEG.CTF2.Core.Commands.Data;
using System.Collections.Specialized;
using System.Text;

namespace NEG.CTF2.Core.Commands;

// IMPORTANT:
// In net 9.0 a new type 'StringKeyedDictionary' will be implemented
// This allows ReadOnlySpan<char> dictionary lookups
// In future, switch to net9 on full release
// And refactor to support 'StringKeyedDictionary'
// This is to allow for minimal allocations
// Applies for 'FormatCommand' too
internal sealed class ColourCommand : ICommand
{
	private static readonly Dictionary<ColourGround, Dictionary<string, int>> groundToPredefinedColourMapper = new()
	{
		{ 
			ColourGround.Foreground,  
			new()
			{ 
				{ "Black", 30 },
				{ "Red", 31 },
				{ "Green", 32 },
				{ "Yellow", 33 },
				{ "Blue", 34 },
				{ "Magenta", 35 },
				{ "Cyan", 36 },
				{ "Default", 39 },
			} 
		},
		{
			ColourGround.Background,
			new()
			{
				{ "Black", 40 },
				{ "Red", 41 },
				{ "Green", 42 },
				{ "Yellow", 43 },
				{ "Blue", 44 },
				{ "Magenta", 45 },
				{ "Cyan", 46},
				{ "White", 47 },
				{ "Default", 49 },
			}
		}
	};

	/// <param name="_ground">Forground, or background</param>
	/// <param name="_colour">Either a RGB value (R: x, Y: g, B: z) or fixed colour type (Red)</param>
	public ColourCommand(scoped ReadOnlySpan<char> _ground, scoped ReadOnlySpan<char> _colour)
	{
		Ground = _ground switch
		{
			"FG" => ColourGround.Foreground,
			"BG" => ColourGround.Background,
			_ => throw new Exception($"Colour ground specified does not one of: [\"FG\", \"BG\"]")
		};
		var _colourMapper = groundToPredefinedColourMapper[Ground];
		if(!_colourMapper.TryGetValue(_colour.ToString(), out var _colourSequence))
		{
			throw new Exception($"Inputted colour '{_colour}', is not a valid colour. " +
				$"Perhaps you meant one of the following?\n{ShowBySimilarity(Ground, _colour)}");
		}
		EscapeSequence = ICommand.Format(_colourSequence);
	}
	public ColourGround Ground { get; }
	public string EscapeSequence { get; }

	private static string ShowBySimilarity(ColourGround _ground, scoped ReadOnlySpan<char> _invalidColour)
	{
		var _builder = new StringBuilder();
		foreach(var _colour in groundToPredefinedColourMapper[_ground])
		{
			if(_invalidColour.CompareTo(_colour.Key, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_builder.AppendLine($"-{_colour.Key}");
			}
		}
		return _builder.ToString();
	}
}
