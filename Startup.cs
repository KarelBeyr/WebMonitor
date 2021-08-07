using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMonitor
{
    public static class DataModel
    {
        public static Dictionary<string, string> Dict = new Dictionary<string, string>();
        public static DateTime Started = DateTime.Now;
    }
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("", async context =>
                {
                    var body = new StreamReader(context.Request.Body);
                    var requestBody = await body.ReadToEndAsync();
                    var sb = new StringBuilder();
                    sb.AppendLine($"Log from {DataModel.Started}");
                    foreach (var kv in DataModel.Dict)
                    {
                        sb.AppendLine($"{kv.Key}: {kv.Value}");
                    }
                    await context.Response.WriteAsync(sb.ToString());
                });
                endpoints.MapPost("/reset", async context =>
                {
                    DataModel.Dict.Clear();
                    DataModel.Started = DateTime.Now;
                    await context.Response.WriteAsync($"reset");
                });
                endpoints.MapPost("/{name:alpha}", async context =>
                {
                    var body = new StreamReader(context.Request.Body);
                    var requestBody = await body.ReadToEndAsync();
                    var fn = context.Request.Path.ToString().Substring(1);
                    DataModel.Dict[fn] = requestBody;
                    await context.Response.WriteAsync($"message received");
                });
            });
        }
    }
}
