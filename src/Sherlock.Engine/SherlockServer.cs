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
        private readonly IDataEngine _data;
        public SherlockServer(string host, int port)
        {
            _data = new DataEngine();
            _server = new Grpc.Core.Server()
            {
                Services =
                {
                    Services.SherlockService.BindService(new SherlockService(_data))
                },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            _server.Start();
        }

        public void Dispose()
        {
            _server.ShutdownAsync().Wait();
            _data.Dispose();
        }

        public IDataEngine DataEngine => _data;
    }
}
