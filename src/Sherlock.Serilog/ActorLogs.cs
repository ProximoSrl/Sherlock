using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Proto;
using Serilog.Events;
using Sherlock.Support;

namespace Sherlock.Serilog
{
    public static class ActorLogs
    {
        private const int QueueLen = 100;
        private static readonly ConcurrentDictionary<string, FixedLenQueueEx<TrackedLog>> Messages;

        static ActorLogs()
        {
            Messages = new ConcurrentDictionary<string, FixedLenQueueEx<TrackedLog>>();
        }

        public static void Track(LogEvent logEvent, IFormatProvider formatProvider)
        {
            if (logEvent.Properties.TryGetValue("ActorId", out LogEventPropertyValue v))
            {
                var key = ((ScalarValue)v).Value.ToString().ToLowerInvariant();
                var q = Messages.GetOrAdd(key, new FixedLenQueueEx<TrackedLog>(QueueLen));
                var message = logEvent.RenderMessage(formatProvider);
                q.Add(i=> new TrackedLog(
                    i,
                    logEvent.Properties["SourceContext"].ToString(),
                    logEvent.Level.ToString(), 
                    logEvent.Timestamp,
                    message
                ));
            }
        }

        public static IEnumerable<TrackedLog> LogsOf(string actorId)
        {
            var id = actorId.ToLowerInvariant();
            if (Messages.TryGetValue(id, out FixedLenQueueEx<TrackedLog> messages))
            {
                return messages.Reverse();
            }
            
            return new TrackedLog[0];
        }
    }
}