using Proto;
using Proto.Schedulers.SimpleScheduler;
using Sherlock.Client;
using Sherlock.ProtoActor;
using Sherlock.ProtoActor.Messages;

namespace Sherlock.DemoApp.Engine
{
    public class DemoEngine : IDemoEngine
    {
        private readonly PID _root;
        private readonly PID _sherlock;

        public DemoEngine(ISherlockClient client)
        {
            var props = Actor.FromProducer(() => new DemoRootActor());
            _root = Actor.SpawnNamed(props, "Demo");

            _sherlock = Actor.SpawnNamed
            (
                Actor.FromProducer
                (() => new ProtoActor.SherlockInspectionActor(
                       new SimpleScheduler(),
                       client,
                       new SherlockInspectionOptions()
                   )
                ), 
                "Sherlock"
            );

            _sherlock.Tell(new AddToInspection()
            {
                ActorId = _root
            });
        }

        public void Dispose()
        {
            _root.Stop();
        }
    }
}
