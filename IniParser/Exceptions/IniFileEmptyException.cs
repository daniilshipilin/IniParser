namespace IniFileParser.Exceptions;

using System;

public class IniFileEmptyException : Exception
{
    public IniFileEmptyException(string message)
        : base(message)
    {
    }
}
