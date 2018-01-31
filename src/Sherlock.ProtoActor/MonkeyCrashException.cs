using System;
using System.Runtime.Serialization;

namespace Sherlock.ProtoActor
{
    public class MonkeyCrashException : Exception
    {
        public MonkeyCrashException()
        {
        }

        public MonkeyCrashException(string message) : base(message)
        {
        }

        public MonkeyCrashException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MonkeyCrashException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}