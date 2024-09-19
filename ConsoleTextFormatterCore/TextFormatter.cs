using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NEG.CTF2.Core.Commands;

namespace NEG.CTF2.Core;

public sealed class TextFormatter
{
	public TextFormatter(string _text)
	{
		text = _text;
	}

	private readonly string text;

	public ReadOnlySpan<char> Text
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return text;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GenerateFormat()
	{
		var _textSpan = Text;
		var _indexes = _textSpan.IndexesOfAll('[');

		var _builder = new StringBuilder();
		foreach(var _index in _indexes)
		{
			var _commandSeperators = GetCommandSeperatorIndexes(Text, _index, out int _commandEndIndex);
			var _regions = CreateCommandRegions(_commandSeperators, _textSpan, _index, _commandEndIndex);
			var _commands = CreateContextualisedCommands(_textSpan, _regions);
			// TODO (TextFormatter)
			// Iterate _textSpan
			// Use _index and _commandEndIndex (_to) to remove the command, to do this:
			// If there is no text before the _index (IE '[' is the first char in the span)
			// then loop the _commands, and add their EscapeSequences to the stringbuilder.
			// After skip to next index where you use the index from the previous _commandEndIndex
			// and get a slice from the endIndex to the _index.
			// After, add this region to the stringBuilder,
			// then do a loop over the _commands, and add their EscapeSequences to the stringbuilder. 
		}
		// Return stringbuilder.ToString();
		return string.Empty;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ReadOnlySpan<CommandRegion> CreateContextualisedCommands(scoped in ReadOnlySpan<char> _textSpan, 
		scoped ReadOnlySpan<Range> _regions)
	{
		var _commandRegions = new CommandRegion[_regions.Length];
		for(int i = 0; i < _regions.Length; i++)
		{
			var _region = _regions[i];
			if(IsColourCommand(in _textSpan, _region, out var _ground, out var _colour))
			{
				_commandRegions[i] = new CommandRegion(new ColourCommand(_ground, _colour), _region);
				continue;
			}
			_commandRegions[i] = default;
		}
		return [];
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsColourCommand(scoped in ReadOnlySpan<char> _textSpan, Range _range,
		out ReadOnlySpan<char> _ground, out ReadOnlySpan<char> _colour)
	{
		var _start = _range.Start.Value;
		var _end = _range.End.Value;
		var _scope = _textSpan[_start.._end];
		var _colourSeperatorIndex = _scope.IndexOf(':');

		if(_colourSeperatorIndex == -1)
		{
			_ground = [];
			_colour = [];
			return false;
		}
		_ground = _scope[.._colourSeperatorIndex];
		_colour = _scope[_colourSeperatorIndex..];
		return true;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ReadOnlySpan<Range> CreateCommandRegions(scoped ReadOnlySpan<int> _commandSeperatorIndexes, 
		scoped in ReadOnlySpan<char> _textSpan, 
		int _from,
		int _to)
	{
		var _commandRegions = new Range[_commandSeperatorIndexes.Length + 1].AsSpan();

		var _start = 1;
		var _end = _commandSeperatorIndexes[0];
		(_start, _end) = GetNonWhitespaceRange(_textSpan, _start, _end);
		_commandRegions[0] = new Range(_start, _end);
		List<string> _tmp = new();
		for(int i = 1; i < _commandSeperatorIndexes.Length; i++)
		{
			var _region = ReadOnlySpan<char>.Empty;
			_start = _commandSeperatorIndexes[i - 1] + 1;
			_end = _commandSeperatorIndexes[i];
			(_start, _end) = GetNonWhitespaceRange(_textSpan, _start, _end);
			_commandRegions[i] = new Range(_start, _end);
		}
		_start = _commandSeperatorIndexes[^1] + 1;
		_end = _to;
		(_start, _end) = GetNonWhitespaceRange(_textSpan, _start, _end);
		_commandRegions[^1] = new Range(_start, _end);
		return _commandRegions;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (int _start, int _end) GetNonWhitespaceRange(scoped in ReadOnlySpan<char> _textSpan, 
		int _from, int _to)
	{
		int _localIndex = 0;
		while(_localIndex < _to - _from)
		{
			if(_textSpan[_localIndex] is ' ')
			{
				_from++;
				_localIndex++;
				continue;
			}
			break;
		}
		_localIndex = _textSpan.Length - 1;
		while(_localIndex > 0)
		{
			if(_textSpan[_localIndex] is ' ')
			{
				_to--;
				_localIndex--;
				continue;
			}
			break;
		}
		return (_from, _to);
	}
	// TODO: Validation
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ThrowIfInvalidCommand(scoped in ReadOnlySpan<char> _text)
	{

	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ReadOnlySpan<int> GetCommandSeperatorIndexes(scoped in ReadOnlySpan<char> _text, int _from,
		out int _to)
	{
		int _localIndex = _from + 1;
		var _commandSeperatorIndexList = new List<int>();
		while(_localIndex < _text.Length && _text[_localIndex] is not ']')
		{
			if(_text[_localIndex] is not ',')
			{
				_localIndex++;
				continue;
			}
			_commandSeperatorIndexList.Add(_localIndex);
			_localIndex++;
		}
		if(_localIndex == _text.Length - 1 && _text[_localIndex] is not ']')
		{
			throw new Exception($"{nameof(_text)} does not contain a valid command end char from the last declaration of the command." +
				$"");
		}
		_to = _localIndex;
		if(_commandSeperatorIndexList.Count > 0)
		{
			return CollectionsMarshal.AsSpan(_commandSeperatorIndexList);
		}
		if(_localIndex - _from == 1)
		{
			throw new Exception($"{nameof(text)} contains no recognised command decimator, " +
				$"please use ',' as the command seperator");
		}
		// Is the command region empty?
		// If so, throw
		else if(_text.Slice(_from + 1, _localIndex - _from - 1).IsWhiteSpace())
		{
			throw new Exception($"{nameof(text)} contains no recognised command decimator, " +
				$"please use ',' as the command seperator");
		}
		return CollectionsMarshal.AsSpan(_commandSeperatorIndexList);
	}
}
