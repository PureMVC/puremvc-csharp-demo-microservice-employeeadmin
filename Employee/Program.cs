//
//  Program.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using Employee.View.Components;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Employee
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