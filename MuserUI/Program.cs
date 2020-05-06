using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Tolltech.MuserUI
{
    public class Program
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(Program));  

        public static void Main(string[] args)
        {
            Console.WriteLine($"Start muser with args {string.Join(", ", args)}");

            var log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);


            log.Info("Log configured");

            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var config = new ConfigurationBuilder()
                        //.AddJsonFile("hosting.json", optional: true) //this is not needed, but could be useful
                        .AddCommandLine(args)
                        .Build();

                    webBuilder
                        .UseConfiguration(config)
                        .UseStartup<Startup>();
                });
    }
}
