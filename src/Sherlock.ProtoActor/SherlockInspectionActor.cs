using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Schedulers.SimpleScheduler;
using Sherlock.Client;
using Sherlock.Messages;
using Sherlock.Services;

namespace Sherlock.ProtoActor
{
    public class SherlockInspectionActor : IActor
    {
        private readonly ISimpleScheduler _scheduler;
        private readonly InspectionReportMap _reports = new InspectionReportMap();
        private CancellationTokenSource _cts;
        public static PID Pid { get; private set; }
        private readonly HashSet<PID> _targets = new HashSet<PID>();
        private readonly ISherlockClient _client;
        private readonly ILogger _logger = Proto.Log.CreateLogger<SherlockInspectionActor>();

        public SherlockInspectionActor(ISimpleScheduler scheduler, ISherlockClient client)
        {
            _scheduler = scheduler;
            _client = client;
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                {
                    SherlockInspectionActor.Pid = context.Self;
                    _scheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(5),
                        context.Self,
                        Inspect.Instance,
                        out _cts
                    );
                    break;
                }

                case Stopping _:
                {
                    _cts.Cancel();
                    break;
                }

                case AddToInspection add:
                {
                    _targets.Add(add.Actor);
                    break;
                }

                case Inspect i:
                {
                    if (!_targets.Any())
                    {
                        _logger.LogWarning("No targets configued");
                        return;
                    }

                    _logger.LogDebug("Sending inspection reports");
                    // send reports
                    try
                    {
                        await _client.PushAsync(_reports.Reports.Values).ConfigureAwait(false);
                        _logger.LogDebug("Reports sent");
                    }
                    catch (RpcException ex )
                    {
                        _logger.LogDebug("Sherlock server connection error: {message}", ex.Message);
                        if (ex.Status.StatusCode != StatusCode.Unavailable)
                        {
                            throw;
                        }
                    }

                    // refresh reports
                    _logger.LogDebug("Creating new inspection reports");
                    foreach (var t in _targets)
                    {
                        t.Request(Inspect.Instance, context.Self);
                    }

                    break;
                }

                case InspectionReportRequest req:
                {
                    context.Sender.Tell(_reports);
                    break;
                }

                case InspectionReport report:
                {
                    _reports.Reports[report.ActorId] = report;
                    break;
                }
            }
        }
    }
}
