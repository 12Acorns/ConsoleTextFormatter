using NEG.CTF2.Core.Utility;
using System;

namespace NEG.CTF2.Core.Commands;

internal sealed class CommandRegion
{
    public CommandRegion(ICommand _command, Range _segment)
    {
        Command = _command;
        Segment = _segment;
    }
    public ICommand Command { get; }
    public Range Segment { get; }
}
