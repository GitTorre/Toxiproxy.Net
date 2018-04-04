using System;
using System.Diagnostics;
using Toxiproxy;
using Toxiproxy.Net;
using Toxiproxy.Net.Toxics;

namespace DriverClient
{
    internal class Program
    {
        protected static Connection _connection;
        protected static Process _process;

        private static void Main(string[] args)
        {
            InitProxy();
            StartProxy();
            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
            StopProxy();
            if (!_process.HasExited)
            {
                _process?.Kill();
            }
        }

        private static void InitProxy()
        {
            _process = new Process()
            {
                StartInfo = new ProcessStartInfo(@"..\..\..\compiled\Win64\toxiproxy-server-2.1.2-windows-amd64.exe")
            };
            _process.Start();
            _connection = new Connection();
        }

        private static void StartProxy()
        {
            var client = _connection.Client();

            var proxy = new Proxy
            {
                Name = "localToGoogle",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com:443"
            };

            var newProxy = client.Add(proxy);

            var toxic = new LatencyToxic
            {
                Name = "LatencyToxicTest",
                Stream = ToxicDirection.DownStream,
                Attributes =
                {
                    Jitter = 10,
                    Latency = 3000
                }
            };
            var newToxic = newProxy.Add(toxic);
        }

        private static void StopProxy()
        {
            var connection = new Connection();
            var client = connection.Client();
            var proxy = client.FindProxy("localToGoogle");

            proxy.Enabled = false;
            proxy.Update();
        }
    }
}
