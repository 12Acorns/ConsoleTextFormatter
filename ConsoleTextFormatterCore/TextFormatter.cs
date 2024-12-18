using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NEG.CTF2.Core.Commands;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace NEG.CTF2.Core;

public sealed class TextFormatter
{
	private const string RESETSEQUENCE = "\x1B[0m";

	/// <exception cref="ArgumentNullException"></exception>
	public TextFormatter(string _text, FormattingRules _rules)
	{
		if(_rules is null)
		{
			throw new ArgumentNullException(nameof(_rules));
		}

		text = _text;
		rules = _rules;
	}
	public TextFormatter(string _text) : this(_text, new FormattingRules()) { }

	private readonly string text;
	private readonly FormattingRules rules;

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GenerateFormat()
	{
		Debug.Assert(ConsoleStateManager.IsVirtual);
		var _textSpan = text.AsSpan();
		if(!ConsoleStateManager.IsVirtual)
		{
			return RemoveCommands(in _textSpan);
		}
		if(_textSpan.IsWhiteSpace() || _textSpan.Length is 0)
		{
			return string.Empty;
		}

		var _commandIdentifierIndexes = _textSpan.IndexesOfAll('[');
		if(_commandIdentifierIndexes.Length is 0)
		{
			return text;
		}

		var _builder = new StringBuilder(_textSpan.Length);
		foreach(var _commandIdentifierIndex in _commandIdentifierIndexes)
		{
			var (_commandStartIndex, _commandEndIndex, _nextCommandStartOrEndOfStringIndex) =
				GetCommandIndexex(_textSpan, _commandIdentifierIndex);
			var _commands = GetCommands(_textSpan, _commandStartIndex + 1, _commandEndIndex - 1);
			AppendCommands(_commands, in _builder);

			var _sliceBeginIndex = _commandEndIndex + 1;
			if(ContainsEscapeSequence(_textSpan, _commandEndIndex + 1, rules.CharCutoff))
			{
				_sliceBeginIndex += rules.CharCutoff;
			}

			_builder.Append(_textSpan[_sliceBeginIndex.._nextCommandStartOrEndOfStringIndex ]);
		}

		return _builder.Append(RESETSEQUENCE).ToString();
	}
	private static ReadOnlySpan<CommandRegion> GetCommands(scoped in ReadOnlySpan<char> _text, 
		int _commandStartIndex, int _commandEndIndex)
	{
		var _seperators = _text.IndexesOfAll(',', _commandStartIndex, _commandEndIndex);
		var _regions = new Range[_seperators.Length + 1];
		int _previousRegionEndIndex = _commandStartIndex;
		for(int i = 0; i < _seperators.Length; i++)
		{
			_regions[i] = new Range(_previousRegionEndIndex, _seperators[i] - 1);
			_previousRegionEndIndex = _seperators[i] + 1;
		}
		_regions[^1] = new Range(_previousRegionEndIndex, _commandEndIndex);
		return CreateContextualisedCommands(in _text, _regions);
	}
	private static (int CommandStartIndex, int CommandEndIndex, int nextCommandStartOrStringEndIndex) GetCommandIndexex(
		scoped in ReadOnlySpan<char> _text, int _commandStartIndex)
	{
		if(!TryGetIndexOfFirst(_text, ']', out var _endCommandIndex, _commandStartIndex))
		{
			throw new Exception("Invalid command entered, could not find command end character: ']'");
		}
		if(!TryGetIndexOfFirst(_text, '[', out var _nextCommandIndex, _commandStartIndex + 1))
		{
			return (_commandStartIndex, _endCommandIndex, _text.Length - 1);
		}
		return (_commandStartIndex, _endCommandIndex, _nextCommandIndex);
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ContainsEscapeSequence(scoped in ReadOnlySpan<char> _text, int _from, int _checkLength)
	{
		for(int i = 0; i < _checkLength; i++)
		{
			var _actualIndex = i + _from;
			if(_actualIndex >= _text.Length)
			{
				return false;
			}
			if(IsEscapeSequence(_text[_actualIndex]))
			{
				return true;
			}
		}
		return false;
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
			TryGetIndexOfFirst(_textSpan, ']', out var _commandEndIndex, _index);

			if(_index is 0)
			{
				_previousEndIndex = _commandEndIndex + 1 + rules.CharCutoff;
				continue;
			}
			var _slice = _textSpan[_previousEndIndex.._index];
			_builder.Append(_slice);
			_previousEndIndex = _commandEndIndex + 1 + rules.CharCutoff;
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
			var (_sliceStart, _sliceEnd) = GetNonWhitespaceRange(in _textSpan, _region.Start.Value, _region.End.Value + 1);
			var _slice = _textSpan[_sliceStart.._sliceEnd];
			_commandRegions[i] = new CommandRegion(new FormatCommand(_slice), _region);
		}
		return _commandRegions;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsColourCommand(scoped in ReadOnlySpan<char> _textSpan, Range _range,
		out ReadOnlySpan<char> _ground, out ReadOnlySpan<char> _colour)
	{
		var _start = _range.Start.Value;
		var _end = _range.End.Value + 1;
		// end is exclusive, hence +1
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
	private static bool TryGetIndexOfFirst(scoped in ReadOnlySpan<char> _text, char _target, out int _index, int _from = 0)
	{
		for(int i = _from; i < _text.Length; i++)
		{
			if(_text[i] == _target)
			{
				_index = i;
				return true;
			}
		}
		_index = -1;
		return false;
	}
	// TODO: Validation
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ThrowIfInvalidCommand(scoped in ReadOnlySpan<char> _text)
	{

	}
}