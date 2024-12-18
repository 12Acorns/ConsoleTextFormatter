using NEG.CTF2.Core.Utility;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NEG.CTF2.Core;

internal static class SpanExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<int> IndexesOfAll(this scoped ReadOnlySpan<char> _span, char _char, 
		int _from, int _to)
	{
		// if size (in bytes) of the array of possible int indexes is > 1024 (stack size) do a heap allocation instead, else
		// do a stack allocation
		var _listOfIndexes = new ListSpan<int>(
			_span.Length * sizeof(int) > 1024
			? new int[_span.Length]
			: stackalloc int[_span.Length]);

		for(int i = _from; i < _to; i++)
		{
			if(_span[i] == _char)
			{
				_listOfIndexes.TryAdd(i);
				continue;
			}
		}
		return _listOfIndexes.ToArray().AsSpan();
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<int> IndexesOfAll(this scoped ReadOnlySpan<char> _span, char _char)
	{
		// if size (in bytes) of the array of possible int indexes is > 1024 (stack size) do a heap allocation instead, else
		// do a stack allocation
		var _listOfIndexes = new ListSpan<int>(
			_span.Length * sizeof(int) > 1024 
			? new int[_span.Length] 
			: stackalloc int[_span.Length]);

		for(int i = 1; i < _span.Length; i += 2)
		{
			if(_span[i - 1] == _char)
			{
				_listOfIndexes.TryAdd(i - 1);
				continue;
			}
			if(_span[i] == _char)
			{
				_listOfIndexes.TryAdd(i);
				continue;
			}
		}
		if(_span[^1] == _char && _listOfIndexes[^1] != _span.Length - 1)
		{
			_listOfIndexes.TryAdd(_span.Length - 1);
		}

		return _listOfIndexes.ToArray().AsSpan();
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<int> IndexesOfAll(this scoped Span<char> _span, char _char) =>
		IndexesOfAll((ReadOnlySpan<char>)_span, _char);
}
