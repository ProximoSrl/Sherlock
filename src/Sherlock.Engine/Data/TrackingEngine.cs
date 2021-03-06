﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using Sherlock.Engine.Messages;
using Sherlock.Services;

namespace Sherlock.Engine.Data
{
    public class TrackingEngine : ITrackingEngine
    {
        private readonly ConcurrentDictionary<string, PID> _roots = new ConcurrentDictionary<string, PID>();
        private readonly Props _props;

        public TrackingEngine()
        {
            _props = Actor.FromProducer(() => new ClientRoot());
        }

        public void Dispose()
        {
            foreach (var pid in _roots.Values)
            {
                pid.Stop();
            }
        }

        public void ProcessSingle(string clientId, object message)
        {
            var target = _roots.GetOrAdd(clientId, CreateRoot(clientId));
            target.Tell(message);
        }

        public void ProcessBatch(string clientId, IEnumerable<object> messages)
        {
            try
            {
                var target = _roots.GetOrAdd(clientId, CreateRoot);
                foreach (var m in messages)
                {
                    target.Tell(m);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private PID CreateRoot(string clientId)
        {
            return Actor.Spawn(_props);
        }

        public string[] GetClients() => _roots.Keys.ToArray();

        public async Task<TrackedStateMap> GetReportAsync(string clientId)
        {
            if (clientId != null && _roots.TryGetValue(clientId, out var node))
            {
                return await node.RequestAsync<TrackedStateMap>(new QueryReports()).ConfigureAwait(false);
            }

            var report = new TrackedStateMap();

            var inspectionReport = new TrackedState
            {
                ActorId = "demoapp/demo"
            };

            inspectionReport.InternalState.Add("kernel::name", "Name");
            inspectionReport.InternalState.Add("kernel::actorType", "ActorType");
            inspectionReport.InternalState.Add("kernel::status", "Started");

            report.Reports.Add("root", inspectionReport);

            return report;
        }

        public async Task<LogsAndMessagesData> GetDataAsync(string clientId, string actorId)
        {
            if (clientId != null && _roots.TryGetValue(clientId, out var node))
            {
                return await node.RequestAsync<LogsAndMessagesData>(new QueryLogsAndMessages
                {
                    ActorId = actorId
                }).ConfigureAwait(false);
            }

            return new LogsAndMessagesData();
        }

        public void Clear(string clientId)
        {
            if (clientId != null && _roots.TryGetValue(clientId, out var node))
            {
                node.Tell(new ClearRequest());
            }
        }
    }
}