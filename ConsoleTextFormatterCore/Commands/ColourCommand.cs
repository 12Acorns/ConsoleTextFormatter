using NEG.CTF2.Core.Commands.Data;

namespace NEG.CTF2.Core.Commands;

internal sealed class ColourCommand : ICommand
{
	private readonly Dictionary<ColourGround, Dictionary<string, string>> groundToPredefinedColourMapper = new()
	{
		{ 
			ColourGround.Foreground,  
			new()
			{ 
				{ "Black", $"{ICommand.escapeSequencePrefix}30"},
				{ "Red", $"{ICommand.escapeSequencePrefix}31"},
				{ "Green", $"{ICommand.escapeSequencePrefix}32"},
				{ "Yellow", $"{ICommand.escapeSequencePrefix}33"},
				{ "Blue", $"{ICommand.escapeSequencePrefix}34"},
				{ "Magenta", $"{ICommand.escapeSequencePrefix}35"},
				{ "Cyan", $"{ICommand.escapeSequencePrefix}36"},
				{ "Default", $"{ICommand.escapeSequencePrefix}39"},
			} 
		},
		{
			ColourGround.Background,
			new()
			{
				{ "Black", $"{ICommand.escapeSequencePrefix}40"},
				{ "Red", $"{ICommand.escapeSequencePrefix}41"},
				{ "Green", $"{ICommand.escapeSequencePrefix}42"},
				{ "Yellow", $"{ICommand.escapeSequencePrefix}43"},
				{ "Blue", $"{ICommand.escapeSequencePrefix}44"},
				{ "Magenta", $"{ICommand.escapeSequencePrefix}45"},
				{ "Cyan", $"{ICommand.escapeSequencePrefix}46"},
				{ "White", $"{ICommand.escapeSequencePrefix}47"},
				{ "Default", $"{ICommand.escapeSequencePrefix}49"},
			}
		}
	};

	/// <param name="_ground">Forground, or background</param>
	/// <param name="_colour">Either a RGB value (R: x, Y: g, B: z) or fixed colour type (Red)</param>
	public ColourCommand(string _ground, string _colour)
	{
		Ground = _ground switch
		{
			"FG" => ColourGround.Foreground,
			"BG" => ColourGround.Background,
			_ => throw new Exception($"Colour ground specified does not one of: [\"FG\", \"BG\"]")
		};
		Colour = _colour;
	}
	public ColourGround Ground { get; }
	public string Colour { get; }

	public string GetUnderlyingConsoleValue()
	{
		var _colourMapper = groundToPredefinedColourMapper[Ground];
		if(!_colourMapper.TryGetValue(Colour, out var _colour))
		{
			throw new Exception($"Inputted colour '{_colour}', is not a valid colour. Please enter a valid colour.");
		}
		return _colour;
	}
}
