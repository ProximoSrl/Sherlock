using System;
using Serilog.Core;
using Serilog.Events;

namespace Sherlock.Serilog
{
    public class ActorEventSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public ActorEventSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            ActorLogs.Track(logEvent, _formatProvider);
        }
    }
}
