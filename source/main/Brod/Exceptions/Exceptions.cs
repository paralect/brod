using System;

namespace Brod.Exceptions
{
    public class CorruptedMessageException : Exception
    {
        public CorruptedMessageException()
            : base(String.Format(
                "Possibly corrupted message")) { }
    }
}