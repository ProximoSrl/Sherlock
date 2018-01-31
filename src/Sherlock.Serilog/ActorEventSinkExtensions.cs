using System;
using Serilog;
using Serilog.Configuration;

namespace Sherlock.Serilog
{
    public static class ActorEventSinkExtensions
    {
        public static LoggerConfiguration ActorSink(
            this LoggerSinkConfiguration loggerConfiguration,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new ActorEventSink(formatProvider));
        }
    }
}