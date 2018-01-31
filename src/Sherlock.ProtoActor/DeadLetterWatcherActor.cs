using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using Serilog.Context;
using Sherlock.Services;

namespace Sherlock.ProtoActor
{
    public class DeadLetterWatcherActor : AbstractActor
    {
        private long _counter;
        protected override Task OnReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                {
                    EventStream.Instance.Subscribe<DeadLetterEvent>(letter =>
                    {
                        using (LogContext.PushProperty("ActorId", context.Self.ToShortString()))
                        {
                            _counter++;
                            Logger.LogWarning("'{0}' got '{1}:{2}' from '{3}'",
                                letter.Pid.ToShortString(),
                                letter.Message.GetType().Name,
                                letter.Message,
                                letter.Sender?.ToShortString()
                            );
                        }
                    });
                    break;
                }
            }

            return Actor.Done;
        }

        protected override void OnReport(IInspectionReport report)
        {
            report.Guard(() => _counter == 0, $"Found {_counter} dead letters");
        }
    }
}
