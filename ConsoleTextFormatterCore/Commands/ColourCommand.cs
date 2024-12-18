using System.Collections.Specialized;
using NEG.CTF2.Core.Commands.Data;
using System.Text;

namespace NEG.CTF2.Core.Commands;

internal sealed class ColourCommand : ICommand
{
	static ColourCommand()
	{
		groundToPredefinedColourMapper = [];

		var _foreGroundColours = new Dictionary<string, int>()
		{
			{ "Black", 30 },
			{ "Red", 31 },
			{ "Green", 32 },
			{ "Yellow", 33 },
			{ "Blue", 34 },
			{ "Magenta", 35 },
			{ "Cyan", 36 },
			{ "Default", 39 },
		};
		var _backGroundColours = new Dictionary<string, int>()
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
		};

		groundToPredefinedColourMapper.Add(ColourGround.Foreground, _foreGroundColours.GetAlternateLookup<ReadOnlySpan<char>>());
		groundToPredefinedColourMapper.Add(ColourGround.Background, _backGroundColours.GetAlternateLookup<ReadOnlySpan<char>>());
	}

	private static readonly Dictionary<ColourGround, Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>>> groundToPredefinedColourMapper;

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
		if(!_colourMapper.TryGetValue(_colour, out var _colourSequence))
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
		foreach(var _colour in groundToPredefinedColourMapper[_ground].Dictionary)
		{
			if(_invalidColour.CompareTo(_colour.Key, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_builder.AppendLine($"-{_colour.Key}");
			}
		}
		return _builder.ToString();
	}
}
