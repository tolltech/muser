using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;
using Vostok.Logging.Microsoft;

namespace Tolltech.MuserUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Start muser with args {string.Join(", ", args)}");

            var consoleLog = new SynchronousConsoleLog();
            var fileLog = new FileLog(new FileLogSettings
            {
                FilePath = @"logs/logg",
                RollingStrategy = new RollingStrategyOptions
                {
                    Period = RollingPeriod.Day,
                    Type = RollingStrategyType.ByTime,
                    MaxFiles = 7,
                }
            });
            var compositeLog = new CompositeLog(consoleLog, fileLog);

            LogProvider.Configure(compositeLog);
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
                })
                .ConfigureLogging(builder => builder.AddVostok(LogProvider.Get()));
    }
}
