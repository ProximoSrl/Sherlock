using Microsoft.Extensions.Configuration;
using Sherlock.Client;

namespace Sherlock.DemoApp
{
    public class SherlockDemoClientOptions : SherlockClientOptions
    {
        public bool Enabled { get; set; }
        public int FlushIntervalSeconds { get; set; }

        private SherlockDemoClientOptions()
        {
            ClientName = "Demo";
            FlushIntervalSeconds = 5;
            Enabled = false;
        }

        public static SherlockDemoClientOptions LoadFrom(IConfiguration config) { 
            var opts = new SherlockDemoClientOptions();
            config.GetSection("SherlockClientSettings").Bind(opts);
            return opts;
        }
    }
}