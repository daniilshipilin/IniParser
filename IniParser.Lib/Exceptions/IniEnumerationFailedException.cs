using System;

namespace IniParser.Lib.Exceptions;

public class IniEnumerationFailedException : Exception
{
    public IniEnumerationFailedException(string message)
        : base(message)
    {
    }
}
