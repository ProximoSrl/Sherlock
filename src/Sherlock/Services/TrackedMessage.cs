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

        public TrackedMessage(UInt32 position, object message, PID sender, Types.Direction direction)
        {
            millisFromEpoch_ = DateTime.UtcNow.ToEpochMillis();
            Position = position;
            Sender = sender;
            Direction = direction;
            Message.Add("sequence", Position.ToString());

            switch (message)
            {
                //case IChunk chunk:
                //{
                //    Message.Add("type",
                //        $"{chunk.Payload?.GetType()} @{chunk.Position}: {chunk.PartitionId} # {chunk.Index}");
                //    break;
                //}

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

            if (Sender != null)
            {
                Message.Add("sender", Sender.ToShortString());
            }

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

            if (Target != null)
            {
                dic["target"] = Target.ToShortString();
            }

            dic["direction"] = Direction.ToString();

            return dic;
        }
    }
}