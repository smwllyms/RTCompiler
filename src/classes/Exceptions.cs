using System;
using System.Collections.Generic;
using System.Text;

namespace RTCompiler.src.classes
{
    class RTCException : Exception
    {
        private const string BASE = "[RTC Exception] ";
        public RTCException()
        {
        }

        public RTCException(string message)
            : base(BASE + message)
        {
        }

        public RTCException(string message, Exception inner)
            : base(BASE + message, inner)
        {
        }
    }
    // For when References are undefined
    class RTCUndefinedReferenceException : RTCException
    {
        private const string BASE = "Undefined Reference Exception: ";
        public RTCUndefinedReferenceException()
        {
        }

        public RTCUndefinedReferenceException(string message)
            : base(BASE + message)
        {
        }

        public RTCUndefinedReferenceException(string message, Exception inner)
            : base(BASE + message, inner)
        {
        }
    }
    // For when parsing errors occur
    class RTCParsingException : RTCException
    {
        private const string BASE = "Parsing Exception: ";
        public RTCParsingException()
        {
        }

        public RTCParsingException(string message)
            : base(BASE + message)
        {
        }

        public RTCParsingException(string message, Exception inner)
            : base(BASE + message, inner)
        {
        }
    }
}
