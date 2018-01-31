using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Proto;
using Sherlock.Engine.Data;
using Sherlock.Host.Models;
using Sherlock.Messages;
using Sherlock.ProtoActor;
using Sherlock.Serilog;
using Sherlock.Services;

namespace Sherlock.Host.Controllers
{
    [Route("api/[controller]")]
    public class SherlockController : Controller
    {
        private readonly string _host;
        private readonly IDataEngine _engine;
        public SherlockController(IConfiguration configuration, IDataEngine engine)
        {
            _engine = engine;
            this._host = "nonhost:5001";
        }

        [HttpGet("full/{*id}", Name = "ReportByPath")]
        public async Task<Node> Full(string id)
        {
            if (id != null)
            {
                id = System.Web.HttpUtility.UrlDecode(id);
            }

            var clientId = _engine.GetClients().FirstOrDefault();
            var report = await _engine.GetReportAsync(clientId).ConfigureAwait(false);

            var model = new InspectorViewModel(report, x => Url.RouteUrl(
                "ReportByPath",
                new { id = x },
                "",
                Request.Host.ToUriComponent()
            ), false);

            var node = model.ByPath(id);

            if (node != null)
            {
                var data = await _engine.GetDataAsync(clientId, node.Id).ConfigureAwait(false);
                node.Logs = data.Logs;
                node.SetMessages(data.Messages);
            }

            return node;
        }

        [HttpGet("tree/{*id}")]
        public async Task<Node> Tree(string id)
        {
            if (id != null)
            {
                id = System.Web.HttpUtility.UrlDecode(id);
            }

            var report = await _engine.GetReportAsync(_engine.GetClients().FirstOrDefault()).ConfigureAwait(false);

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

        [HttpGet("messages/{*id}")]
        public object Messages(string id)
        {
            if (id != null)
            {
                id = System.Web.HttpUtility.UrlDecode(id);
            }

            return ActorLogs.LogsOf(id).ToArray();
        }
    }
}