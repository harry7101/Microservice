using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;

namespace hotel_base
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("app init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    var localconfig = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                    var consul_server = localconfig["consul_server"];

                    webBuilder.ConfigureAppConfiguration((ctx, cfg) => {
                      
                      
                        cfg.AddConsul("hoteljson", op =>
                        {
                            op.ConsulConfigurationOptions = cco =>
                            {
                                cco.Address = new Uri(consul_server);

                            };
                            op.ReloadOnChange = false;//是否热更新

                        });

                        localconfig = cfg.Build();
                        var port = localconfig["serviceInfo:port"];


                        webBuilder.ConfigureKestrel(options =>
                        {
                            options.ListenAnyIP(int.Parse(port));
                        });
                    });
           
                   
                    webBuilder.UseStartup<Startup>();
                   // webBuilder.UseUrls("http://*:6003");
                })
            
                .UseNLog();
    }
}
