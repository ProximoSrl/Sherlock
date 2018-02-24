using System;
using System.Collections.Generic;
using System.Linq;
using Sherlock.Services;
using TrackedLog = Sherlock.Services.TrackedLog;

namespace Sherlock.Host.Models
{
    public class Node
    {
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly List<Node> _childNodes = new List<Node>();

        public string Id { get; private set; }
        public NavigationLink Parent { get; set; }
        public NavigationLink[] ChildsLinks { get; set; }

        public IDictionary<string, string> Status { get; private set; }
        public IList<IDictionary<string, string>> TrackedMessages { get; private set; }
        public IList<string> Warnings { get; private set; }
        public IEnumerable<Node> ChildsNodes { get; private set; }
        public DateTime Timestamp { get; private set; }
        public IEnumerable<TrackedLog> Logs { get; set; }

        public Node(string id, bool recursive)
        {
            Id = id;
            Status = new Dictionary<string, string>();
            TrackedMessages = new List<IDictionary<string, string>>();
            Timestamp = DateTime.Now;
            this.Warnings = new string[0];

            if (recursive)
            {
                ChildsNodes = _childNodes;
            }
        }

        public Node(TrackedState report, bool recursive) : this(report.ActorId, recursive)
        {
            Timestamp = UnixEpoch.AddMilliseconds(report.MillisFromEpoch).ToLocalTime();

            foreach (var keyValuePair in report.Status)
            {
                this.Status[keyValuePair.Key] = keyValuePair.Value;
            }

            this.Warnings = report.Warnings.ToList();
        }

        public void Add(Node node)
        {
            _childNodes.Add(node);
        }

        public void CreateNodeLinks(Func<string, string> urlmapper)
        {
            ChildsLinks = _childNodes
                .Select(x => new NavigationLink(x.Id, urlmapper(x.Id)))
                .ToArray();
        }

        public void SetMessages(IEnumerable<TrackedMessage> messages)
        {
            foreach (var msg in messages)
            {
                TrackedMessages.Add(msg.ToDictionary());
            }
        }
    }
}