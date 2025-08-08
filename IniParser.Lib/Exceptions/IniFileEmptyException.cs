using System;

namespace IniParser.Lib.Exceptions;

public class IniFileEmptyException : Exception
{
    public IniFileEmptyException(string message)
        : base(message)
    {
    }
}
