//
//  Service.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Role.View.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Role.View.Components
{
    public class Service
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            var address = serverAddressesFeature.Addresses.First();
            int port = int.Parse(address.Split(':').Last());
            
            Environment.SetEnvironmentVariable("SERVICE_PORT", port.ToString());
            Environment.SetEnvironmentVariable("APPLICATION_NAME", env.ApplicationName);

            ApplicationFacade.GetInstance("service").Startup(this);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.Run(async context =>
            {
                await Delegate.Service(context);
            });
        }

        public void Result(HttpContext context, int status, object resultData)
        {
            context.Response.StatusCode = status;

            try
            {
                switch (context.Request.Path.Value)
                {
                    case "/health":
                        break;

                    default:
                        var match = Regex.Match(context.Request.Path.Value, @"/employees/(\d+)/roles"); // employees/:id/roles
                        if (match.Success)
                        {
                            if (context.Request.Method == "GET")
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.WriteAsync(JsonConvert.SerializeObject(resultData));
                            } 
                            else if (context.Request.Method == "PUT")
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.WriteAsync(JsonConvert.SerializeObject(resultData));
                            }
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void Fault(HttpContext context, int status, object exception)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            
            if (exception is Exception)
            {
                context.Response.WriteAsync(JsonConvert.SerializeObject(new {Code = status, ((Exception) exception).Message}));
            }
            else
            {
                context.Response.WriteAsync(JsonConvert.SerializeObject(exception));
            }
        }
        
        public IService Delegate { get; set; }
    }
}