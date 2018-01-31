using System.Threading.Tasks;
using Proto;
using Sherlock.ProtoActor;

namespace Sherlock.DemoApp.Engine
{
    public class DemoChildActor : AbstractActor
    {
        protected override Task OnReceiveAsync(IContext context)
        {
            return Actor.Done;
        }
    }
}