using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sherlock.Engine;

namespace Sherlock.Host
{
    public class SherlockOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public static SherlockOptions FromConfig(IConfiguration config)
        {
            var options = new SherlockOptions();
            config.GetSection("Sherlock").Bind(options);
            return options;
        }
    }

    public class Program
    {
        private static IConfiguration _config;

        public static void Main(string[] args)
        {
            _config = LoadConfig();
            var options = SherlockOptions.FromConfig(_config);

            Console.WriteLine($"Starting gRPC @ {options.Host}:{options.Port}");
            using (var s = new SherlockServer(options.Host, options.Port))
            {
                BuildWebHost(args, s).Run();
            }
        }

        public static IWebHost BuildWebHost(string[] args, SherlockServer server)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(_config)
                .ConfigureServices(services => services.AddSingleton(server.TrackingEngine))
                .UseStartup<Startup>()
                .Build();
        }

        private static IConfiguration LoadConfig()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }
    }
}