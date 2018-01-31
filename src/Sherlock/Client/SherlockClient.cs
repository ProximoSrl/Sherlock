using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Sherlock.Services;

namespace Sherlock.Client
{
    public interface ISherlockClient : IDisposable
    {
        Task<TrackLogsResponse> PushAsync(IEnumerable<LogMessage> logs);
        Task<TrackMessagesResponse> PushAsync(IEnumerable<TrackedMessage> messages);
        Task<TrackInspectionResponse> PushAsync(IEnumerable<InspectionReport> reports);
    }

    public static class SherlockConstants
    {
        public const string ClientId = "client_id";
    }

    public class SherlockClientOptions
    {
        public string ClientName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class SherlockClient : ISherlockClient
    {
        private readonly Channel _channel;
        private readonly SherlockService.SherlockServiceClient _sherlockClient;
        private readonly Metadata _clientId;

        public SherlockClient(SherlockClientOptions options) : this(options.ClientName, options.Host, options.Port)
        {
        }

        public SherlockClient(string name, string host, int port)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (host == null) throw new ArgumentNullException(nameof(host));

            _channel = new Channel(
                host,
                port,
                ChannelCredentials.Insecure
            );
            _sherlockClient = new SherlockService.SherlockServiceClient(_channel);
            _clientId = new Metadata { { SherlockConstants.ClientId, name } };
        }

        public async Task<TrackLogsResponse> PushAsync(IEnumerable<LogMessage> logs)
        {
            using (var c = _sherlockClient.TrackLogs(_clientId))
            {
                var batch = new LogBatch();
                batch.Messages.Add(logs);

                await c.RequestStream.WriteAsync(batch).ConfigureAwait(false);
                await c.RequestStream.CompleteAsync().ConfigureAwait(false);
                return await c.ResponseAsync.ConfigureAwait(false);
            }
        }

        public async Task<TrackMessagesResponse> PushAsync(IEnumerable<TrackedMessage> messages)
        {
            using (var c = _sherlockClient.TrackMessages(_clientId))
            {
                var batch = new MessagesBatch();
                batch.Messages.Add(messages);

                await c.RequestStream.WriteAsync(batch).ConfigureAwait(false);
                await c.RequestStream.CompleteAsync().ConfigureAwait(false);
                return await c.ResponseAsync.ConfigureAwait(false);
            }
        }

        public async Task<TrackInspectionResponse> PushAsync(IEnumerable<InspectionReport> reports)
        {
            using (var c = _sherlockClient.TrackInspection(_clientId))
            {
                foreach (var i in reports)
                {
                    await c.RequestStream.WriteAsync(i).ConfigureAwait(false);
                }

                await c.RequestStream.CompleteAsync().ConfigureAwait(false);
                return await c.ResponseAsync.ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _channel.ShutdownAsync().Wait();
        }
    }
}
