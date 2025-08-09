using System;

namespace IniParser.Lib.Exceptions;

public class IniFileEmptyException(string message) : Exception(message)
{
}
