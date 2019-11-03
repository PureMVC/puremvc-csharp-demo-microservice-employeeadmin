//
//  Service.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Collections.Generic;
using System.Text.Json;
using Department.View.Interfaces;
using Department.Model.ValueObject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Department.View.Components
{
    public class Service
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            App = app;
            Env = env;

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
                    case "/departments":
                        context.Response.ContentType = "application/json";
                        context.Response.WriteAsync(JsonSerializer.Serialize((List<DepartmentVO>) resultData));
                        break;

                    case "/health":
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

            context.Response.WriteAsync(exception is Exception ex
                ? JsonSerializer.Serialize(new {code = status, message = ex.Message})
                : JsonSerializer.Serialize(exception));
        }
        
        public IApplicationBuilder App { get; set; }
        
        public IWebHostEnvironment Env { get; set; }
        
        public IService Delegate { get; set; }
    }
}