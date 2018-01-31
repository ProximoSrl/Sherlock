using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Proto;
using Proto.Schedulers.SimpleScheduler;
using Sherlock.Client;
using Sherlock.Messages;
using Sherlock.ProtoActor.Messages;
using Sherlock.Services;
using Sherlock.Support;

namespace Sherlock.ProtoActor
{
    public static class ActorMessages
    {
        private static ISherlockClient SherlockClient { get; set; }
        private const int QueueLen = 100;
        private static ConcurrentDictionary<string, FixedLenQueueEx<TrackedMessage>> Messages;
        private static PID _reporter;

        static ActorMessages()
        {
            Messages = new ConcurrentDictionary<string, FixedLenQueueEx<TrackedMessage>>();
        }

        public static void Start(ISherlockClient client)
        {
            SherlockClient = client;

            _reporter = Actor.SpawnNamed(
                Actor.FromProducer(() => new BackgroundMessagesReporter(client)),
                "sherlock-reporter"
            );
        }

        public static TrackedMessage Receive(IContext context)
        {
            if (context.Message is Inspect)
                return null;

            var actorId = context.Self.ToShortString();
            var q = Messages.GetOrAdd(actorId, new FixedLenQueueEx<TrackedMessage>(QueueLen));
            return q.Add(i => new TrackedMessage(actorId, i, context.Message, context.Sender?.ToShortString(), null, TrackedMessage.Types.Direction.In));
        }

        public static IEnumerable<TrackedMessage> MessagesOf(string actorId)
        {
            if (Messages.TryGetValue(actorId, out FixedLenQueueEx<TrackedMessage> messages))
            {
                return messages.Reverse();
            }

            return new TrackedMessage[0];
        }

        public static TrackedMessage Send(ISenderContext senderContext, PID target, MessageEnvelope messageEnvelope)
        {
            if (senderContext is IContext context)
            {
                var key = context.Self.ToShortString();
                var q = Messages.GetOrAdd(key, new FixedLenQueueEx<TrackedMessage>(QueueLen));
                return q.Add(i => new TrackedMessage(key, i, messageEnvelope.Message, context.Sender?.ToShortString(), target?.ToShortString(), TrackedMessage.Types.Direction.Out));
            }

            return null;
        }

        public static void Push(TrackedMessage tracking)
        {
            if (tracking != null)
            {
                _reporter?.Tell(tracking);
            }
        }
    }

    public class BackgroundMessagesReporter : IActor
    {
        internal class Flush
        {
            internal static readonly Flush Instance = new Flush();
        }

        private readonly ISherlockClient _client;
        private readonly ISimpleScheduler _scheduler;
        private IList<TrackedMessage> _buffer;
        private CancellationTokenSource _cts;

        public BackgroundMessagesReporter(ISherlockClient client)
        {
            _client = client;
            _scheduler = new SimpleScheduler();
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                {
                    _scheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(5),
                        context.Self,
                        Flush.Instance,
                        out _cts
                    );
                    break;
                }

                case Stopping _:
                {
                    _cts.Cancel();
                    break;
                }

                case TrackedMessage tracked:
                {
                    if (_buffer == null)
                    {
                        _buffer = new List<TrackedMessage>(1000);
                    }

                    _buffer.Add(tracked);

                    break;
                }

                case Flush _ when _buffer != null:
                {
                    _client.PushAsync(_buffer);
                    _buffer = null;
                    break;
                }
            }
            return Actor.Done;
        }
    }
}