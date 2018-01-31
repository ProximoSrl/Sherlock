using System;
using System.Collections.Generic;
using System.Text;
using Proto;

namespace Sherlock.ProtoActor
{
    public static class PropsExtensions
    {
        public static Props WithSherlock(this Props props)
        {
            return props.WithSenderMiddleware(next => (sendContext, target, messageEnvelope) =>
                    {
                        ActorMessages.Push(ActorMessages.Send(sendContext, target, messageEnvelope));
                        return next(sendContext, target, messageEnvelope);
                    });
        }
    }
}
