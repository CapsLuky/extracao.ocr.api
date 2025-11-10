using Microsoft.AspNetCore.Hosting;
using log4net;
using log4net.Config;
using System;
using System.IO;

namespace API.Contratual.WebApi;

public class Program
{
    //private static readonly log4net.ILog _log = LogManager.GetLogger(typeof(Program));
    
    public static void Main(string[] arg)
    {
        XmlConfigurator.Configure(new FileInfo("log4net.config"));
        
        CreateHostBuilder(arg).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}

