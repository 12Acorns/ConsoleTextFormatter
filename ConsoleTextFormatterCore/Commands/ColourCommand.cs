using NEG.CTF2.Core.Commands.Data;

namespace NEG.CTF2.Core.Commands;

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
	public ColourCommand(ReadOnlySpan<char> _ground, ReadOnlySpan<char> _colour)
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
				"Please enter a valid colour.");
		}
		EscapeSequence = ICommand.Format(_colourSequence);
	}
	public ColourGround Ground { get; }
	public string EscapeSequence { get; }
}
