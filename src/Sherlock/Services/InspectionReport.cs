using System;
using Proto;
using Sherlock.Support;

namespace Sherlock.Services
{
    public interface IInspectionReport
    {
        IInspectionReport Add(string key, string value);
        IInspectionReport Add(string key, TimeSpan timeSpan);
        IInspectionReport Add(string key, long counter);
        IInspectionReport Add(string key, bool value);
        IInspectionReport Guard(Func<bool> conditions, string message);
    }
    
    public partial class InspectionReport : IInspectionReport
    {
        public static InspectionReport Create(IContext context)
        {
            var report = new InspectionReport
            {
                Pid = context.Self,
                MillisFromEpoch = DateTime.UtcNow.ToEpochMillis()
            };

            report.Childs.AddRange(context.Children);

            return report;
        }

        public IInspectionReport Add(string key, string value)
        {
            this.Status[key] = value;
            return this;
        }

        public IInspectionReport Add(string key, TimeSpan timeSpan)
        {
            return Add(key, timeSpan.ToString());
        }

        public IInspectionReport Add(string key, long counter)
        {
            return Add(key, counter.ToString());
        }

        public IInspectionReport Add(string key, bool value)
        {
            return Add(key, value.ToString());
        }

        public IInspectionReport Guard(Func<bool> conditions, string message)
        {
            if (!conditions())
            {
                this.Warnings.Add(message);
            }

            return this;
        }
    }
}