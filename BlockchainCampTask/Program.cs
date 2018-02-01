using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlockchainCampTask.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlockchainCampTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //if(!File.Exists(String.Format("./blocks/{0}.json", "Status")))
            //{
            //    Status status = new Status();
            //    status.last_hash = "0";
            //    status.neighbours = new List<Link>();
            //    using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", "Status")))
            //    {
            //        JsonSerializer serializer = new JsonSerializer();
            //        serializer.Serialize(file, status);
            //    }
            //}

            if (!File.Exists(String.Format("./blocks/{0}.json", "Links")))
            {
                List<Link> links = new List<Link>();
               
                using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", "Links")))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, links);
                }
            }

            Console.WriteLine("Test");
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                    .UseUrls("http://192.168.44.66:8770")
                .Build();
    }
}
