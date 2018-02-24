using System;
using System.Collections.Generic;
using System.Linq;
using Sherlock.Services;

namespace Sherlock.Host.Models
{
    public class NodeViewModelBuilder
    {
        public Node Node { get; private set; }

        public NodeViewModelBuilder(
            string nodeId,
            TrackedStateMap map,
            Func<string, string> urlmapper,
            Action<Node> action = null
            )
        {
            var found = map.Reports.Values.FirstOrDefault(x=> x.ActorId == nodeId);

            if (found != null)
            {
                this.Node = new Node(found, false);
            }
            else
            {
                var parentOf = map.Reports.Values.FirstOrDefault(x => x.Childs.Contains(nodeId));
                if (parentOf != null)
                {
                    this.Node = new GhostNode(nodeId);
                }
            }

            action?.Invoke(this.Node);
        }
    }

    public class InspectorViewModel
    {
        private readonly Node _root;
        private readonly IDictionary<string, Node> _map = new Dictionary<string, Node>();

        public Node Root => _root;

        public InspectorViewModel(
            TrackedStateMap map,
            Func<string, string> urlmapper,
            bool recursive,
            Action<Node> action = null
        )
        {
            _root = new Node("actors", recursive);

            foreach (var report in map.Reports)
            {
                var node = new Node(report.Value, recursive);
                _map[node.Id] = node;

                action?.Invoke(node);
            }

            foreach (var node in _map.Values)
            {
                var last = node.Id.LastIndexOf('/');

                // root?
                if (last == -1 || last == node.Id.IndexOf('/'))
                {
                    _root.Add(node);
                }
                else
                {
                    var parent = node.Id.Substring(0, last);
                    node.Parent = new NavigationLink(parent, urlmapper(parent));
                    _map[parent].Add(node);
                }
            }

            foreach (var node in _map.Values.Union(new[] { _root }))
            {
                node.DetectGhosts(urlmapper);
                node.CreateNodeLinks(urlmapper);
            }
        }

        public Node ByPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (_map.ContainsKey(path))
                {
                    return _map[path];
                }
            }

            return Root;
        }
    }
}