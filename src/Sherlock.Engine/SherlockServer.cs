using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Proto;
using Sherlock.Engine.Data;
using Sherlock.Services;
using SherlockService = Sherlock.Engine.RpcServices.SherlockService;

namespace Sherlock.Engine
{
    public class SherlockServer : IDisposable
    {
        private readonly Server _server;
        private readonly ITrackingEngine _tracking;
        public SherlockServer(string host, int port)
        {
            _tracking = new TrackingEngine();
            _server = new Grpc.Core.Server()
            {
                Services =
                {
                    Services.SherlockService.BindService(new SherlockService(_tracking))
                },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            _server.Start();
        }

        public void Dispose()
        {
            _server.ShutdownAsync().Wait();
            _tracking.Dispose();
        }

        public ITrackingEngine TrackingEngine => _tracking;
    }
}
