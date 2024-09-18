using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
	public string GenerateFormat()
	{
		var _textSpan = Text;
		var _indexes = _textSpan.IndexesOfAll('[');

		foreach(var _index in _indexes)
		{
			var _commandSeperators = GetCommandSeperatorIndexes(Text, _index, out int _commandEndIndex);
			var _regions = CreateCommandRegions(_commandSeperators, _textSpan, _index, _commandEndIndex);
			var _commands = CreateContextualisedCommands(_textSpan, _regions);

		}
		return string.Empty;
	}

	private static ReadOnlySpan<CommandRegion> CreateContextualisedCommands(ReadOnlySpan<char> textSpan, ReadOnlySpan<Range> regions)
	{
		throw new NotImplementedException();
	}

	private static ReadOnlySpan<Range> CreateCommandRegions(scoped ReadOnlySpan<int> _commandSeperatorIndexes, 
		ReadOnlySpan<char> _textSpan, 
		int _from,
		int _to)
	{
		var _commandRegions = new Range[_commandSeperatorIndexes.Length + 1].AsSpan();
		for(int i = 0; i < _commandSeperatorIndexes.Length; i++)
		{
			var _region = ReadOnlySpan<char>.Empty;
			var _start = _from + 1;
			var _end = 0;
			if(i == 0)
			{
				_end = _commandSeperatorIndexes[0];
				_commandRegions[i] = new Range(_start, _end);
				continue;
			}
			if(i == _commandSeperatorIndexes.Length - 1)
			{
				_start = _commandSeperatorIndexes[^1] + 1;
				_end = _to;
				_commandRegions[i] = new Range(_start, _end);
				break;
			}
			_start = _commandSeperatorIndexes[i] + 1;
			_end = _commandSeperatorIndexes[i + 1];
			_commandRegions[i] = new Range(_start, _end);
		}
		return _commandRegions;
	}
	// TODO: Validation
	private static void ThrowIfInvalidCommand(scoped ReadOnlySpan<char> _text)
	{

	}
	private static ReadOnlySpan<int> GetCommandSeperatorIndexes(scoped ReadOnlySpan<char> _text, int _from,
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
