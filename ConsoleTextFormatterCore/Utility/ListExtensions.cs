namespace NEG.CTF2.Core.Utility;

internal static class ListExtensions
{
	internal static void ClearTo<T>(this List<T> _this, List<T> _target)
	{
		_target.AddRange(_this);
		_this.Clear();
	}
}