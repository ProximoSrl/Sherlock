using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Proto;
using Sherlock.Support;

namespace Sherlock.Services
{
    public partial class TrackedMessage
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public TrackedMessage(string actorId, UInt64 position, object message, PID sender, PID target, Types.Direction direction)
        {
            millisFromEpoch_ = DateTime.UtcNow.ToEpochMillis();
            ActorId = actorId;
            Sequence = position;
            Direction = direction;
            Message.Add("sequence", Sequence.ToString());

            if (sender != null)
            {
                Sender = sender.ToShortString();
                Message.Add("sender", Sender);
            }

            if (target != null)
            {
                Target = target.ToShortString();
                Message.Add("target", Target);
            }

            switch (message)
            {
                case Terminated terminated:
                {
                    Message.Add("type", $"{message.GetType().FullName} => {terminated.Who.ToShortString()}");
                    break;
                }

                default:
                {
                    Message.Add("type", message.GetType().FullName);
                    break;
                }
            }

            Message.Add("when", EpochUtils.FromMillis(MillisFromEpoch).ToString("O"));

            try
            {
                Message.Add("message", JsonConvert.SerializeObject(message, JsonSerializerSettings));
            }
            catch (Exception ex)
            {
                Message.Add("message", $"Error serializing message : {ex.Message}");
            }
        }

        public IDictionary<string, string> ToDictionary()
        {
            var dic = new Dictionary<string, string>();

            foreach (var (k, v) in Message)
            {
                dic[k] = v;
            }

            dic["direction"] = Direction.ToString();

            return dic;
        }
    }
}