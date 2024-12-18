using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Numerics;
using System.Linq;
using System.Text;
using System;
using System.Runtime.CompilerServices;

namespace NEG.CTF2.Core.Utility;
internal ref struct ListSpan<TSource>
{
	/// <summary>
	/// <paramref name="_source"/> is a empty span/unintiialized span from <paramref name="_from"/>
	/// </summary>
	public ListSpan(Span<TSource> _source, int _fromIndex)
	{
		if(_source.Length < 1)
		{
			throw new ArgumentException(nameof(_source));
		}
		if(_fromIndex >= _source.Length)
		{
			throw new ArgumentException(nameof(_source));
		}

		elementsSpan = _source;
		internalIndex = Start = _fromIndex;
	}
	/// <summary>
	/// <paramref name="_source"/> is a empty span/unintiialized span
	/// </summary>
	public ListSpan(Span<TSource> _source)
	{
		if(_source.Length < 1)
		{ 
			throw new ArgumentException(nameof(_source));
		}

		elementsSpan = _source;
		internalIndex = Start = 0;
	}
	public ListSpan(int _length)
	{
		if(_length < 1)
		{
			throw new ArgumentException(nameof(_length));
		}

		elementsSpan = new TSource[_length];
		internalIndex = Start = 0;
	}
	public static unsafe ListSpan<TUnmanaged> CreateUnmanaged<TUnmanaged>(TUnmanaged* _ptr, int _from, int _length)
		where TUnmanaged : unmanaged
	{
		if(_length < 1)
		{
			throw new ArgumentException(nameof(_length));
		}
		if(_from >= _length)
		{
			throw new ArgumentException(nameof(_from));
		}
		if(typeof(TUnmanaged) != typeof(TSource))
		{
			throw new Exception(nameof(TUnmanaged) + " is not " + nameof(TSource));
		}

		return new ListSpan<TUnmanaged>(new Span<TUnmanaged>(_ptr, _length), _from);
	}
	public static unsafe ListSpan<TUnmanaged> CreateUnmanaged<TUnmanaged>(TUnmanaged* _ptr, int _length)
		where TUnmanaged : unmanaged
	{
		if(_length < 1)
		{
			throw new ArgumentException(nameof(_length));
		}
		if(typeof(TUnmanaged) != typeof(TSource))
		{
			throw new Exception(nameof(TUnmanaged) + " is not " + nameof(TSource));
		}

		return new ListSpan<TUnmanaged>(new Span<TUnmanaged>(_ptr, _length));
	}

	public int Count => internalIndex - Start;
	public int Start { get; private set; }

	private readonly Span<TSource> elementsSpan;
	private int internalIndex;

	private readonly List<int> removedIndexes = new();

	public bool TryAdd(TSource _value)
	{
		if(internalIndex == elementsSpan.Length)
		{
			return false;
		}
		if(removedIndexes.Count > 0)
		{
			elementsSpan[removedIndexes[0]] = _value;
			removedIndexes.RemoveAt(0);
			return true;
		}
		elementsSpan[internalIndex++] = _value;
		return true;
	}
	public bool TryRemove(TSource _value)
	{
		if(!Contains(_value, out var _index))
		{
			return false;
		}
		elementsSpan[_index] = default!;
		removedIndexes.Add(_index);
		return true;
	}
	public TSource[] ToArray(ToArrayOption _option = ToArrayOption.TrimmedSpan)
	{
		switch(_option)
		{
			case ToArrayOption.FullSpan:
				return elementsSpan.ToArray();
			case ToArrayOption.StartToEndIndex:
				return elementsSpan.Slice(Start, Count).ToArray();
			case ToArrayOption.TrimmedSpan:
				if(removedIndexes.Count == 0)
				{
					goto case ToArrayOption.StartToEndIndex;
				}

				var _length = Count - removedIndexes.Count;
				if(_length <= 0)
				{
					return [];
				}
				var _elements = new TSource[_length];
				int _elementsIndex = 0;
				for(int i = Start; i <= internalIndex; i++)
				{
					if(removedIndexes.Contains(i))
					{
						continue;
					}
					_elements[_elementsIndex++] = elementsSpan[i];
				}
				return _elements;
			default:
				throw new ArgumentOutOfRangeException($"{_option} is not yet supported or invalid");
		}
	}

	private bool Contains(TSource _value, out int _index)
	{
		if(_value == null)
		{
			_index = -1;
			return false;
		}

		int _internalIndex = internalIndex;
		if(internalIndex == elementsSpan.Length)
		{
			_internalIndex--;
		}

		for(int i = Start; i <= _internalIndex; i++)
		{
			var _element = elementsSpan[i];
			if(_element == null)
			{
				continue;
			}

			if(_element is IComparable<TSource> _genericComparableElement)
			{
				if(_genericComparableElement != (IComparable<TSource>)_value!)
				{
					continue;
				}
				_index = i;
				return true;
			}
			if(_element is IComparer<TSource> _genericComparerElement)
			{
				if(_genericComparerElement != (IComparer<TSource>)_value!)
				{
					continue;
				}
				_index = i;
				return true;
			}
			if(_element is IComparable _comparableElement)
			{
				if(_comparableElement.CompareTo(_value!) != 0)
				{
					continue;
				}
				_index = i;
				return true;
			}
			if(_element is IComparer _comparerElement)
			{
				if(_comparerElement != (IComparer)_value!)
				{
					continue;
				}
				_index = i;
				return true;
			}
			if(!_element.Equals(_value))
			{
				continue;
			}
			_index = i;
			return true;
		}
		_index = -1;
		return false;
	}

	public TSource this[int _index]
	{
		get
		{
			if(internalIndex == elementsSpan.Length && _index >= internalIndex)
			{
				throw new ArgumentOutOfRangeException(nameof(_index));
			}
			if(_index > internalIndex || _index < Start)
			{
				throw new ArgumentOutOfRangeException(nameof(_index));
			}
			if(removedIndexes.Contains(_index))
			{
				throw new ArgumentOutOfRangeException(nameof(_index));
			}
			return elementsSpan[_index];
		}
	}
}
