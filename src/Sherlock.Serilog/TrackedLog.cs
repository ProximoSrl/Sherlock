using System;

namespace Sherlock.Serilog
{
    public sealed class TrackedLog
    {
        public TrackedLog(long sequence, string logger, string level, DateTimeOffset timestamp, string message)
        {
            Sequence = sequence;
            Message = message;
            Level = level;
            Timestamp = timestamp;
            Logger = logger;
        }

        public long Sequence { get; }
        public string Logger { get; }
        public string Message { get; }
        public string Level { get; }
        public DateTimeOffset Timestamp { get; }
    }
}