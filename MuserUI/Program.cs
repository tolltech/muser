using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Tolltech.MuserUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Start muser with args {string.Join(", ", args)}");

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
