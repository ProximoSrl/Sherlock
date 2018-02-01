using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Proto;
using Proto.Schedulers.SimpleScheduler;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Sherlock.Client;
using Sherlock.Services;

namespace Sherlock.Serilog
{
    public static class SherlockEventSinkExtensions
    {
        public static LoggerConfiguration SherlockSink(
            this LoggerSinkConfiguration loggerConfiguration,
            ISherlockClient client,
            int flushTimeout,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new SherlockEventSink(client, flushTimeout, formatProvider));
        }
    }

    public class SherlockEventSink : ILogEventSink
    {
        private readonly PID _reporter;
        public SherlockEventSink(ISherlockClient client, int flushTimeout, IFormatProvider formatProvider)
        {
            _reporter = Actor.Spawn(Actor.FromProducer(() => new SherlockBackgroundLogReporter(client, formatProvider, flushTimeout)));
        }

        public void Emit(LogEvent logEvent)
        {
            _reporter.Tell(logEvent);
        }
    }

    public class SherlockBackgroundLogReporter : IActor
    {
        private class Flush
        {
            internal static readonly Flush Instance = new Flush();
        }

        private readonly ISherlockClient _client;
        private readonly IFormatProvider _formatProvider;
        private readonly int _flushInterval;
        private readonly ISimpleScheduler _simpleScheduler = new SimpleScheduler();
        private CancellationTokenSource _cts;
        private readonly Dictionary<string, ulong> _messageCounter = new Dictionary<string, ulong>();
        private IList<Services.TrackedLog> _buffer;

        public SherlockBackgroundLogReporter(
            ISherlockClient client,
            IFormatProvider formatProvider,
            int flushInterval
        )
        {
            _client = client;
            _formatProvider = formatProvider;
            _flushInterval = flushInterval;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                {
                    _simpleScheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(_flushInterval),
                        context.Self,
                        Flush.Instance,
                        out _cts
                    );
                    break;
                }

                case Stopping _:
                {
                    _cts.Cancel();
                    BackgroundFlush();

                    break;
                }

                case Flush _:
                {
                    BackgroundFlush();
                    break;
                }

                case LogEvent logEvent:
                {
                    BufferEvent(logEvent);
                    break;
                }
            }

            return Actor.Done;
        }

        private void BackgroundFlush()
        {
            if (_buffer != null)
            {
                _client.PushAsync(_buffer);
                _buffer = null;
            }
        }

        private void BufferEvent(LogEvent logEvent1)
        {
            if (!logEvent1.Properties.TryGetValue("ActorId", out LogEventPropertyValue v))
                return;

            var actorId = ((ScalarValue)v).Value.ToString();

            if (_buffer == null)
            {
                _buffer = new List<Services.TrackedLog>();
            }

            _messageCounter.TryGetValue(actorId, out var last);
            last++;
            _messageCounter[actorId] = last;

            _buffer.Add(new Services.TrackedLog()
            {
                Sequence = last,
                Timestamp = Timestamp.FromDateTimeOffset(logEvent1.Timestamp),
                ActorId = actorId,
                LogType = ConvertLogLevel(logEvent1.Level),
                Text = logEvent1.RenderMessage(_formatProvider),
                Logger = logEvent1.Properties["SourceContext"].ToString()
            });
        }

        private Services.TrackedLog.Types.LogType ConvertLogLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Verbose:
                    return Services.TrackedLog.Types.LogType.Verbose;
                case LogEventLevel.Debug:
                    return Services.TrackedLog.Types.LogType.Debug;
                case LogEventLevel.Information:
                    return Services.TrackedLog.Types.LogType.Information;
                case LogEventLevel.Warning:
                    return Services.TrackedLog.Types.LogType.Warning;
                case LogEventLevel.Error:
                    return Services.TrackedLog.Types.LogType.Error;
                case LogEventLevel.Fatal:
                    return Services.TrackedLog.Types.LogType.Fatal;
            }

            throw new InvalidOperationException();
        }
    }
}