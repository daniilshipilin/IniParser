namespace IniParser.Library.Exceptions
{
    using System;

    public class IniEnumerationFailedException : Exception
    {
        public IniEnumerationFailedException(string message)
            : base(message)
        {
        }
    }
}
