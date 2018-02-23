using System;
using System.Collections.Generic;
using System.Linq;
using Sherlock.Support;

namespace Sherlock.Services
{
    public interface ITrackedState
    {
        ITrackedState Add(string key, string value);
        ITrackedState Add(string key, TimeSpan timeSpan);
        ITrackedState Add(string key, long counter);
        ITrackedState Add(string key, bool value);
        ITrackedState Add(string key, DateTime value);
        ITrackedState Guard(Func<bool> conditions, string message);
    }
    
    public partial class TrackedState : ITrackedState
    {
        public static TrackedState Create(string actorId, IEnumerable<string> childs)
        {
            var report = new TrackedState
            {
                ActorId = actorId,
                MillisFromEpoch = DateTime.UtcNow.ToEpochMillis()
            };

            report.Childs.AddRange(childs);

            return report;
        }

        public ITrackedState Add(string key, string value)
        {
            this.Status[key] = value;
            return this;
        }

        public ITrackedState Add(string key, TimeSpan timeSpan)
        {
            return Add(key, timeSpan.ToString());
        }

        public ITrackedState Add(string key, long counter)
        {
            return Add(key, counter.ToString());
        }

        public ITrackedState Add(string key, bool value)
        {
            return Add(key, value.ToString());
        }

        public ITrackedState Add(string key, DateTime value)
        {
            return Add(key, value.ToString("O"));
        }

        public ITrackedState Guard(Func<bool> conditions, string message)
        {
            if (!conditions())
            {
                this.Warnings.Add(message);
            }

            return this;
        }
    }
}