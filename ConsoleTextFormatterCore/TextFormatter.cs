using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NEG.CTF2.Core.Commands;

namespace NEG.CTF2.Core;

public sealed class TextFormatter
{
	public TextFormatter(string _text, FormattingRules? _rules = default)
	{
		text = _text;
		if(_rules is null)
		{
			_rules = new();
		}
		charCutoff = _rules.CharCutoff;
	}

	private readonly string text;
	private readonly int charCutoff = 2;

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
		Debug.Assert(ConsoleStateManager.IsVirtual);
		var _textSpan = Text;
		if(!ConsoleStateManager.IsVirtual)
		{
			return RemoveCommands(in _textSpan);
		}
		var _indexes = _textSpan.IndexesOfAll('[');
		if(_indexes.Length is 0)
		{
			return text;
		}

		var _builder = new StringBuilder();
		var _previousEndIndex = 0;
		foreach(var _index in _indexes)
		{
			var _commandSeperators = GetCommandSeperatorIndexes(Text, _index, out int _commandEndIndex);
			var _regions = CreateCommandRegions(_commandSeperators, _textSpan, _index, _commandEndIndex);
			var _commands = CreateContextualisedCommands(_textSpan, _regions);

			if(_index is 0)
			{
				_previousEndIndex = _commandEndIndex + 1;
				if(_previousEndIndex + 1 < _textSpan.Length)
				{
					if(IsEscapeSequence(_textSpan[_previousEndIndex]) || 
					   IsEscapeSequence(_textSpan[_previousEndIndex + 1]))
					{
						_previousEndIndex += charCutoff;
					}
				}

				AppendCommands(in _commands, in _builder);
				continue;
			}
			var _slice = _textSpan[_previousEndIndex.._index];
			_builder.Append(_slice);
			AppendCommands(in _commands, in _builder);
			_previousEndIndex = _commandEndIndex + 1;
			if(_previousEndIndex + 1 < _textSpan.Length)
			{
				if(IsEscapeSequence(_textSpan[_previousEndIndex]) ||
				   IsEscapeSequence(_textSpan[_previousEndIndex + 1]))
				{
					_previousEndIndex += charCutoff;
				}
			}
		}
		if(_indexes.Length is > 1)
		{
			return _builder.Append("\x1B[0m").ToString();
		}
		// Remove from main scope to allow reuse of _slice
		{
			var _slice = _textSpan[_previousEndIndex..];
			_builder.Append(_slice);
			return _builder.Append("\x1B[0m").ToString();
		}
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsEscapeSequence(char _char) => _char switch
	{
		'\a' // Bell/alert
	 or '\b' // Backspace
	 or '\f' // Form feed
	 or '\n' // Newline
	 or '\r' // Carriage return
	 or '\t' // Horizontal tab
	 or '\v' // Vertical tab
	 or '\\' // Backslash
	 or '\"' // Double quote
	 or '\'' // Single quote
	 or '\0' /* Null */ => true,
		_ => false
	};
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	private string RemoveCommands(scoped in ReadOnlySpan<char> _textSpan)
	{
		var _indexes = _textSpan.IndexesOfAll('[');
		if(_indexes.Length is 0)
		{
			return text;
		}

		var _builder = new StringBuilder();
		var _previousEndIndex = 0;
		foreach(var _index in _indexes)
		{
			var _commandSeperators = GetCommandSeperatorIndexes(Text, _index, out int _commandEndIndex);

			if(_index is 0)
			{
				_previousEndIndex = _commandEndIndex + 1 + charCutoff;
				continue;
			}
			var _slice = _textSpan[_previousEndIndex.._index];
			_builder.Append(_slice);
			_previousEndIndex = _commandEndIndex + 1 + charCutoff;
		}
		if(_indexes.Length is > 1)
		{
			return _builder.ToString();
		}
		// Remove from main scope to allow reuse of _slice
		{
			var _slice = _textSpan[_previousEndIndex..];
			_builder.Append(_slice);
			return _builder.ToString();
		}
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AppendCommands(scoped in ReadOnlySpan<CommandRegion> _commands, in StringBuilder _builder)
	{
		foreach(var _command in _commands)
		{
			_builder.Append(_command.Command?.EscapeSequence);
		}
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
			var _slice = _textSpan[_region.Start.Value.._region.End.Value];
			_commandRegions[i] = new CommandRegion(new FormatCommand(_slice), _region);
		}
		return _commandRegions;
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
		var (_colourStart, _) = GetNonWhitespaceRange(in _scope, _colourSeperatorIndex + 1, _scope.Length);
		_colour = _scope[_colourStart..];
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
		var _end = 0;
		if(_commandSeperatorIndexes.Length is 0)
		{
			_end = _to;
		}
		else
		{
			_end = _commandSeperatorIndexes[0];
		}

		(_start, _end) = GetNonWhitespaceRange(_textSpan, _start, _end);
		_commandRegions[0] = new Range(_start, _end);
		if(_commandSeperatorIndexes.Length is 0)
		{
			return _commandRegions;
		}

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
		var _scope = _textSpan[_from.._to];
		// The length of _scope
		var _scopeTo = _scope.Length;
		int _localIndex = 0;
		while(_localIndex < _scopeTo)
		{
			if(_scope[_localIndex] is ' ')
			{
				_from++;
				_localIndex++;
				continue;
			}
			break;
		}
		_localIndex = _scope.Length - 1;
		while(_localIndex > 0)
		{
			if(_scope[_localIndex] is ' ')
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
