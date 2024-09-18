using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NEG.CTF2.Core;

internal static class SpanExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<int> IndexesOfAll(this scoped ReadOnlySpan<char> _span, char _char)
	{
		var _listOfIndexes = new List<int>();
		for(int i = 0; i < _span.Length; i++)
		{
			if(_span[i] != _char)
			{
				continue;
			}
			_listOfIndexes.Add(i);
		}
		return CollectionsMarshal.AsSpan(_listOfIndexes);
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<int> IndexesOfAll(this scoped Span<char> _span, char _char)
	{
		var _listOfIndexes = new List<int>();
		for(int i = 0; i < _span.Length; i++)
		{
			if(_span[i] != _char)
			{
				continue;
			}
			_listOfIndexes.Add(i);
		}
		return CollectionsMarshal.AsSpan(_listOfIndexes);
	}
}
