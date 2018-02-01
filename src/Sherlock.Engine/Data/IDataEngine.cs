using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sherlock.Messages;
using Sherlock.Services;

namespace Sherlock.Engine.Data
{
    public interface IDataEngine : IDisposable
    {
        void ProcessSingle(string clientId, object message);
        void ProcessBatch(string clientId, IEnumerable<object> messages);

        string[] GetClients();
        Task<TrackedStateMap> GetReportAsync(string clientId);
        Task<LogsAndMessagesData> GetDataAsync(string clientId, string actorId);
        void Clear(string clientId);
    }
}