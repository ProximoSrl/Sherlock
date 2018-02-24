using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sherlock.Engine.Data;
using Sherlock.Host.Models;
using Sherlock.Serilog;

namespace Sherlock.Host.Controllers
{
    [Route("api/[controller]")]
    public class ActorsController : Controller
    {
        private readonly ITrackingEngine _engine;
        public ActorsController(ITrackingEngine engine)
        {
            _engine = engine;
        }

        [HttpGet("clients")]
        public string[] Clients()
        {
            return _engine.GetClients();
        }

        [HttpGet("full/{clientId}/{*id}", Name = "ReportByPath")]
        public async Task<Node> Full(string clientId, string id)
        {
            if (id != null)
            {
                id = System.Web.HttpUtility.UrlDecode(id);
            }

            clientId = System.Web.HttpUtility.UrlDecode(clientId ?? _engine.GetClients().FirstOrDefault());

            var report = await _engine.GetReportAsync(clientId).ConfigureAwait(false);

            var builder = new NodeViewModelBuilder(id, report, x => Url.RouteUrl(
                "ReportByPath",
                new { id = x },
                "",
                Request.Host.ToUriComponent()
            ));

            var node = builder.Node;

            if (node != null)
            {
                var data = await _engine.GetDataAsync(clientId, node.Id).ConfigureAwait(false);
                node.Logs = data.Logs;
                node.SetMessages(data.Messages);
            }

            return node;
        }

        [HttpGet("tree/{clientId}/{*id}")]
        public async Task<Node> Tree(string clientId, string id)
        {
            if (id != null)
            {
                id = System.Web.HttpUtility.UrlDecode(id);
            }

            clientId = System.Web.HttpUtility.UrlDecode(clientId ?? _engine.GetClients().FirstOrDefault());

            var report = await _engine.GetReportAsync(clientId).ConfigureAwait(false);

            var model = new InspectorViewModel(report, x => Url.RouteUrl(
                "ReportByPath",
                new { id = x },
                "",
                Request.Host.ToUriComponent()
            ), true, node =>
            {
                var logs = ActorLogs.LogsOf(node.Id).ToArray();

                if (logs.Any(x => x.Level == "Warning"))
                {
                    node.Warnings.Add("Warnings found");
                }

                if (logs.Any(x => x.Level == "Error"))
                {
                    node.Warnings.Add("Errors found");
                }
            });

            return model.ByPath(id);
        }
    }
}