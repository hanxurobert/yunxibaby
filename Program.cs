using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;


namespace YunxiBaby {
    public class Program {
        public static void Main (string[] args) {
            var config = new ConfigurationBuilder ()
                .SetBasePath (Directory.GetCurrentDirectory ())
                .AddEnvironmentVariables ()
                .AddJsonFile ("certificate.json", optional : true, reloadOnChange : true)
                .AddJsonFile ($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional : true, reloadOnChange : true)
                .Build ();

            var certificateSettings = config.GetSection ("certificateSettings");
            string certificateFileName = certificateSettings.GetValue<string> ("filename");
            string certificatePassword = certificateSettings.GetValue<string> ("password");

            var certificate = new X509Certificate2 (certificateFileName, certificatePassword);

            CreateWebHostBuilder (args)
                .UseKestrel (options => {
                    options.AddServerHeader = false;
                    options.Listen (IPAddress.Loopback, 5001, listenOptions => {
                        listenOptions.UseHttps (certificate);
                    });
                    options.Listen (IPAddress.Loopback, 5000);
                })
                .UseConfiguration (config)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup>();
    }
}