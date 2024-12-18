namespace NEG.CTF2.Core.Utility;

internal enum ToArrayOption
{
	FullSpan,
	StartToEndIndex,
	/// <summary>
	/// Only elements that are not removed from span and in the start to end range
	/// </summary>
	TrimmedSpan
}
