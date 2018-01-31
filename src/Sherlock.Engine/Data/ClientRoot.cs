﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;
using Sherlock.Engine.Messages;
using Sherlock.Services;
using LogsAndMessagesData = Sherlock.Messages.LogsAndMessagesData;
using QueryLogsAndMessages = Sherlock.Messages.QueryLogsAndMessages;

namespace Sherlock.Engine.Data
{
    public class ClientRoot : IActor
    {
        private readonly IDictionary<string, PID> _twins = new Dictionary<string, PID>();
        private readonly Props _twinProps;
        private InspectionReportMap _map = new InspectionReportMap();

        public ClientRoot()
        {
            _twinProps = Actor.FromProducer(() => new ActorTwin());
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case LogMessage log:
                {
                    GetActor(context, log.ActorId).Tell(log);
                    break;
                }

                case InspectionReport report:
                {
                    _map.Reports[report.ActorId] = report;
                    GetActor(context, report.ActorId).Tell(report);
                    break;
                }

                case TrackedMessage message:
                {
                    GetActor(context, message.ActorId).Tell(message);
                    break;
                }

                case QueryReports req:
                {
                    context.Tell(context.Sender, _map);
                    break;
                }

                case QueryLogsAndMessages q:
                {
                    var data = await context.RequestAsync<LogsAndMessagesData>(GetActor(context, q.ActorId), q).ConfigureAwait(false);
                    context.Tell(context.Sender, data);
                    break;
                }

                case ClearRequest r:
                {
                    foreach (var c in context.Children)
                    {
                        c.Stop();
                    }

                    _twins.Clear();
                    _map = new InspectionReportMap();

                    break;
                }
            }
        }

        private PID GetActor(IContext context, string actorId)
        {
            if (!_twins.TryGetValue(actorId, out var pid))
            {
                pid = context.Spawn(_twinProps);
                _twins.Add(actorId, pid);
            }

            return pid;
        }
    }
}