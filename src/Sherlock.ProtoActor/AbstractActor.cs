using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using Serilog.Context;
using Sherlock.Messages;
using Sherlock.ProtoActor.Messages;
using Sherlock.Services;
using Sherlock.Support;

namespace Sherlock.ProtoActor
{
    public abstract class AbstractActor : IActor
    {
        // ReSharper disable InconsistentNaming
        private readonly DateTime __created = DateTime.UtcNow;
        private long __receivedMessages = 0;
        private long __restartCount = 0;
        private string __currentStatus = "new";
        private string __name;
        private string __group;
        protected ILogger Logger { get; private set; }
        private Random _rnd;
        protected AbstractActor()
        {
            Logger = Proto.Log.CreateLogger(GetType().FullName);
        }

        public Task ReceiveAsync(IContext context)
        {
            using (LogContext.PushProperty("ActorId", context.Self.ToShortString()))
            {
                return SherlockSettings.Enabled ?
                    TrackExecutionAsync(context) :
                    OnReceiveAsync(context);
            }
        }

        private async Task TrackExecutionAsync(IContext context)
        {
            __receivedMessages++;

            var tracking = ActorMessages.Receive(context);
            TryCrash(context);

            if (tracking != null)
            {
                var w = new Stopwatch();
                try
                {
                    w.Start();
                    await OnReceiveAsync(context).ConfigureAwait(false);
                    w.Stop();
                    tracking.Message.Add("executionTimeMs", w.ElapsedMilliseconds.ToString());
                }
                catch (Exception e)
                {
                    tracking.Message.Add("exception", e.GetType().FullName);
                    tracking.Message.Add("exceptionMessage", e.Message);
                    throw;
                }
                ActorMessages.Push(tracking);
            }
            else
            {
                await OnReceiveAsync(context).ConfigureAwait(false);
            }

            SystemHandling(context);
        }

        protected virtual void TryCrash(IContext context)
        {
            if (context.Message is MonkeyCrash crash)
            {
                if (_rnd == null)
                {
                    _rnd = new Random(DateTime.UtcNow.Millisecond);
                }

                var severity = _rnd.Next(100);

                if (severity >= 20)
                {
                    Logger.LogDebug("\n\n\n\n\nBOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM ;-> {message}\n\n\n\n", crash.Message);
                    throw new MonkeyCrashException();
                }
                else if (severity >= 50)
                {
                    foreach (var contextChild in context.Children.OrderBy(x=> _rnd.Next(10)).Take(5))
                    {
                        contextChild.Tell(crash);
                    }
                }
            }
        }

        protected void SetName(string name) => __name = name;
        protected void SetGroup(string @group) => __group = @group;

        private void SystemHandling(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                {
                    Logger.LogDebug("Started");
                    __currentStatus = "Started";
                    __name = context.Self.ExtractName();
                    break;
                }

                case Stopping _:
                {
                    Logger.LogDebug("Stopping");
                    __currentStatus = "Stopping";
                    break;
                }

                case Stopped _:
                {
                    Logger.LogDebug("Stopped");
                    __currentStatus = "Stopped";
                    if (SherlockInspectionActor.Pid != null)
                    {
                        var report = CreateReport(context);
                        SherlockInspectionActor.Pid.Tell(report);
                    }

                    break;
                }

                case Restart _:
                {
                    Logger.LogDebug("Restart");
                    __restartCount++;
                    break;
                }

                case Inspect i:
                {
                    __receivedMessages--;

                    var report = CreateReport(context);

                    context.Sender.Tell(report);

                    break;
                }
            }
        }

        private ITrackedState CreateReport(IContext context)
        {
            foreach (var child in context.Children)
            {
                child.Request(Inspect.Instance, context.Sender);
            }

            var report = TrackedState
                .Create(context.Self.ToShortString(), context.Children.Select(x=>x.ToShortString()))
                .Add("kernel::name", __name)
                .Add("kernel::actorType", GetType().FullName)
                .Add("kernel::status", __currentStatus)
                .Add("kernel::uptime", DateTime.UtcNow - __created)
                .Add("kernel::receivedMessages", __receivedMessages);

            if (__restartCount > 0)
            {
                report.Add("kernel::restarts", __restartCount);
            }

            if (__group != null)
            {
                report.Add("kernel::group", __group);
            }

            ReportState(report);
            return report;
        }

        protected abstract Task OnReceiveAsync(IContext context);

        protected virtual void ReportState(ITrackedState report)
        {
        }
    }
}