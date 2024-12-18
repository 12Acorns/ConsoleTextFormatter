namespace NEG.CTF2.Core.Extensions;

// https://stackoverflow.com/questions/10845820/check-the-length-of-integer-variable/22999111#22999111
internal static class IntExtensions
{
	public static int IntLength(this int i)
	{
		if(i < 0)
			throw new ArgumentOutOfRangeException();
		if(i == 0)
			return 1;
		return (int)Math.Floor(Math.Log10(i)) + 1;
	}
}
