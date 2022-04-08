using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Prometheus.DotNetRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Need to Run Nlog SQL Schema to link to SQL First
            //Otherwise, Programe will throw exception
            try
            {
                Activity.DefaultIdFormat = ActivityIdFormat.W3C;
                NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

                DotNetRuntimeStatsBuilder.Default().StartCollecting();
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        //    DotNetRuntimeStatsBuilder
        //.Customize()
        //.WithContentionStats()
        //.WithJitStats()
        //.WithThreadPoolStats()
        //.WithGcStats()
        //.StartCollecting();
        //    CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
             .UseNLog();  //Nlog Use
    }
}
