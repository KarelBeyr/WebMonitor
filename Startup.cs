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
                    var sb = new StringBuilder();
                    sb.AppendLine($"<p style=\"float:left\" >Log from {DataModel.Started} </p><form style=\"float:left\" action=\"/reset\" method=\"post\"><input type=\"submit\" value=\"reset\"></form>");
                    sb.AppendLine($"<form action=\"/foo{DateTime.Now.Ticks}\" method=\"post\"><input type=\"hidden\" name=\"val\" value=\"xxx\"><input type=\"submit\" value=\"foo\"></form>");

                    foreach (var kv in DataModel.Dict)
                    {
                        sb.AppendLine($"{kv.Key}: {kv.Value} <br>");
                    }
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(sb.ToString());
                });
                endpoints.MapPost("/reset", async context =>
                {
                    DataModel.Dict.Clear();
                    DataModel.Started = DateTime.Now;
                    context.Response.Redirect("/");
                });
                endpoints.MapPost("/{name:required}", async context =>
                {
                    var body = new StreamReader(context.Request.Body);
                    var requestBody = await body.ReadToEndAsync();
                    var fn = context.Request.Path.ToString().Substring(1);
                    DataModel.Dict[fn] = requestBody;
                    //await context.Response.WriteAsync($"message received");
                    context.Response.Redirect("/");
                });
            });
        }
    }
}
