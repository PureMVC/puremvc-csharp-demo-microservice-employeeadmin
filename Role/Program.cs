//
//  Program.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Role.View.Components;

namespace Role
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, config) =>
                {
                    Environment.SetEnvironmentVariable("APPLICATION_NAME", host.HostingEnvironment.ApplicationName);
                })
                .UseStartup<Service>();
    }
}