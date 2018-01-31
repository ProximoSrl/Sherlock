using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Schedulers.SimpleScheduler;
using Sherlock.ProtoActor;

namespace Sherlock.DemoApp.Engine
{
    public class DemoRootActor : AbstractActor
    {
        private readonly ISimpleScheduler _scheduler = new SimpleScheduler();
        private CancellationTokenSource _cts;

        protected override Task OnReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                {
                    var props = Actor.FromProducer(() => new DemoChildActor());
                    context.SpawnNamed(props, "Child1");
                    context.SpawnNamed(props, "Child2");

                    _scheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        context.Self,
                        PingMessage.Instance,
                        out _cts
                    );
                    break;
                }

                case PingMessage p:
                {
                    Logger.LogInformation("Another ping in the wall");
                    break;
                }

                case Stopping _:
                {
                    _cts.Cancel();
                    break;
                }
            }

            return Actor.Done;
        }
    }
}