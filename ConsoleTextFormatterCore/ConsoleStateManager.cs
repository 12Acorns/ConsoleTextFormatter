using System.Runtime.InteropServices;
using System;

public static partial class ConsoleStateManager
{
	/// <summary>
	/// Indicates if virtual termianl processing if possible on current system;
	/// </summary>
	public static bool IsVirtual { get; } = TryInitialise();

	private const int STDOUTPUTHANDLE = -11;
	private const uint ENABLEVIRTUALTERMINALPROCESSING = 4;

	[LibraryImport("kernel32.dll", SetLastError = true)]
	private static partial nint GetStdHandle(int _nStdHandle);

	[LibraryImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool GetConsoleMode(nint _hConsoleHandle, out uint _lpMode);

	[LibraryImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool SetConsoleMode(nint _hConsoleHandle, uint _dwMode);

	private static bool TryInitialise()
	{
		if(!IsValidOperatingSystem())
		{
			return false;
		}

		var _handle = GetStdHandle(STDOUTPUTHANDLE);

		if(_handle == Marshal.GetLastWin32Error())
		{
			return false;
		}

		if(!GetConsoleMode(_handle, out uint _consoleMode))
		{
			return false;
		}

		_consoleMode |= ENABLEVIRTUALTERMINALPROCESSING;

		if(!SetConsoleMode(_handle, _consoleMode))
		{
			return false;
		}
		return true;
	}
	private static bool IsValidOperatingSystem()
	{
		if(!OperatingSystem.IsWindows())
		{
			return false;
		}
		else if(!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10586))
		{
			return false;
		}
		return true;
	}
}