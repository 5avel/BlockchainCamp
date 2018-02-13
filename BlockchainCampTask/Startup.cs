using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockchainCampTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();

                    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

                    
                    Console.WriteLine("{0} Processing request {1}",DateTime.Now.ToString(), context.Request.Path);
                    if(!String.IsNullOrWhiteSpace(context.Request.QueryString.Value))
                        Console.WriteLine("Processing QueryString {0}", context.Request.QueryString.Value);
                    if (!String.IsNullOrWhiteSpace(body))
                    {
                        System.Diagnostics.Debug.Print(body);
                        Console.WriteLine("Processing Body {0}", body);
                    }
                }
               
                await next.Invoke();
            });

            app.UseMvc();
        }
    }
}
