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
        private readonly List<string> _knownChilds;

        public string Id { get; private set; }
        public NavigationLink Parent { get; set; }
        public NavigationLink[] ChildsLinks { get; set; }

        public IDictionary<string, string> InternalState { get; private set; }
        public IList<IDictionary<string, string>> TrackedMessages { get; private set; }
        public IList<string> Warnings { get; private set; }
        public IEnumerable<Node> ChildsNodes { get; private set; }
        public DateTime Timestamp { get; private set; }
        public IEnumerable<TrackedLog> Logs { get; set; }

        public Node(string id, bool recursive)
        {
            Id = id;
            InternalState = new Dictionary<string, string>();
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

            foreach (var keyValuePair in report.InternalState)
            {
                this.InternalState[keyValuePair.Key] = keyValuePair.Value;
            }

            this.Warnings = report.Warnings.ToList();
            this._knownChilds = report.Childs.ToList();
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

        public void DetectGhosts(Func<string, string> urlmapper)
        {
            if (ChildsNodes == null || _knownChilds == null || _knownChilds.Count == 0)
                return;

            var ghosts = _knownChilds.Except(ChildsNodes.Select(x => x.Id));

            foreach (var child in ghosts)
            {
                var ghostChild = new GhostNode(child)
                {
                    Parent = new NavigationLink(this.Id, urlmapper(this.Id))
                };

                Add(ghostChild);
            }
        }
    }

    /// <summary>
    /// Ghost node is a node representing a known actor without tracking infos.
    /// Likely a stalled actor.
    /// </summary>
    public class GhostNode : Node
    {
        public GhostNode(string id) : base(id, false)
        {
            int start = id.LastIndexOf("/");
            if (start != -1)
            {
                InternalState.Add("kernel::name", id.Substring(start + 1));
            }

            InternalState.Add("kernel::actorType", "is.unknown");
            InternalState.Add("kernel::status", "Stalled");
        }
    }
}