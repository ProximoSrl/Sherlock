using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sherlock.Engine.Messages;
using Sherlock.Services;

namespace Sherlock.Engine.Data
{
    public interface ITrackingEngine : IDisposable
    {
        void ProcessSingle(string clientId, object message);
        void ProcessBatch(string clientId, IEnumerable<object> messages);

        string[] GetClients();
        Task<TrackedStateMap> GetReportAsync(string clientId);
        Task<LogsAndMessagesData> GetDataAsync(string clientId, string actorId);
        void Clear(string clientId);
    }
}