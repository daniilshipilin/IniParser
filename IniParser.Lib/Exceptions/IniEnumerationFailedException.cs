using System;

namespace IniParser.Lib.Exceptions;

public class IniEnumerationFailedException(string message) : Exception(message)
{
}
