using System;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using Sherlock.Messages;
using Sherlock.Services;
using Sherlock.Support;

namespace Sherlock.Engine.Data
{
    public class ActorTwin : IActor
    {
        private readonly FixedLenQueue<LogMessage> _logs = new FixedLenQueue<LogMessage>(100);
        private readonly FixedLenQueue<TrackedMessage> _messages = new FixedLenQueue<TrackedMessage>(100);
        private InspectionReport _state;

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case LogMessage log:
                {
                    _logs.Add(log);
                    break;
                }

                case InspectionReport report:
                {
                    _state = report;
                    break;
                }

                case TrackedMessage message:
                {
                    _messages.Add(message);
                    break;
                }

                case QueryLogsAndMessages q:
                {
                    var data = new LogsAndMessagesData();
                    data.Messages.Add(_messages);
                    data.Logs.Add(_logs);

                    context.Tell(context.Sender, data);
                    break;
                }
            }
            return Actor.Done;
        }
    }
}