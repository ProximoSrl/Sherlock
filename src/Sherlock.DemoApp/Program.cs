using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Sherlock.Client;
using Sherlock.DemoApp.Engine;
using Sherlock.ProtoActor;
using Sherlock.Serilog;

namespace Sherlock.DemoApp
{
    public class Program
    {
        private static IConfigurationRoot _config;
        private static ISherlockClient _sherlockClient;

        public static void Main(string[] args)
        {
            LoadConfiguration();

            try
            {
                ConfigureLogging();
                BuildWebHost(args).Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Log.CloseAndFlush();
                _sherlockClient?.Dispose();
            }
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(_config)
                .ConfigureServices(services =>
                    services.AddSingleton<IDemoEngine>(new DemoEngine(_sherlockClient))
                )
                .UseStartup<Startup>()
                .Build();
        }

        private static void LoadConfiguration()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private static void ConfigureLogging()
        {
            var serilogConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(_config);

            var sherlockOptions = SherlockDemoClientOptions.LoadFrom(_config);

            if (sherlockOptions.Enabled)
            {
                SherlockSettings.Enable();

                _sherlockClient = new SherlockClient(sherlockOptions);
                serilogConfig = serilogConfig.WriteTo.SherlockSink(_sherlockClient, sherlockOptions.FlushIntervalSeconds);

                ActorMessages.Start(_sherlockClient);
            }

            Log.Logger = serilogConfig.CreateLogger();
            Proto.Log.SetLoggerFactory(new LoggerFactory().AddSerilog());
        }
    }
}
