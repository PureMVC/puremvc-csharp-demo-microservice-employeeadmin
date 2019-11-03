//
//  StartupCommand.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Role.Model;
using Role.View;
using Role.View.Components;
using Microsoft.AspNetCore.Hosting.Server.Features;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace Role.Controller
{
    public class StartupCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            var serverAddressesFeature = ((Service) notification.Body).App.ServerFeatures.Get<IServerAddressesFeature>();
            var address = serverAddressesFeature.Addresses.First();

            var env = new Dictionary<string, string>
            {
                {"DATABASE_HOST", Environment.GetEnvironmentVariable("DATABASE_HOST")},
                {"DATABASE_NAME", Environment.GetEnvironmentVariable("DATABASE_NAME")},
                {"DATABASE_USER", Environment.GetEnvironmentVariable("DATABASE_USER")},
                {"SA_PASSWORD", Environment.GetEnvironmentVariable("SA_PASSWORD")},
                {"CONSUL_HOST", Environment.GetEnvironmentVariable("CONSUL_HOST")},
                {"APPLICATION_NAME", ((Service) notification.Body).Env.ApplicationName},
                {"SERVICE_PORT", int.Parse(address.Split(':').Last()).ToString()}
            };

            foreach (var (key, value) in env)
            {
                if(value == null) throw new SystemException($@"Please set the {key} in env variables and try again.");
            }

            var connection = $@"
                    Data Source={env["DATABASE_HOST"]};
                    Initial Catalog={env["DATABASE_NAME"]};
                    Persist Security Info=True;
                    User ID={env["DATABASE_USER"]};
                    Password={env["SA_PASSWORD"]};";
            
            Console.WriteLine("Connecting SQL Server.");
            do
            {
                try
                {
                    using var sqlConnection = new SqlConnection(connection);
                    sqlConnection.Open();
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Task.Delay(5000).Wait();
                }
            } while (true);
            
            Console.WriteLine("Connecting Consul.");
            do
            {
                try
                {
                    var service = new
                    {
                        ID = Guid.NewGuid(),
                        Name = env["APPLICATION_NAME"],
                        Address = Dns.GetHostName(),
                        Port = Convert.ToInt32(env["SERVICE_PORT"]),
                        Check = new
                        {
                            Http = "http://" + Dns.GetHostName() + ":" + env["SERVICE_PORT"] + "/health",
                            Interval = "15s"
                        }
                    };
                    
                    var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(service));
                    var request = (HttpWebRequest) WebRequest.Create(env["CONSUL_HOST"] + "/v1/agent/service/register");
                    request.Method = "PUT";
                    request.ContentType = "application/json";
                    request.ContentLength = bytes.Length;
                    
                    using var stream = request.GetRequestStream();
                    stream.Write(bytes, 0, bytes.Length);

                    using var response = (HttpWebResponse) request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK) 
                        break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Consul: " + exception.Message);
                    Task.Delay(5000).Wait();
                }
            } while (true);

            Facade.RegisterCommand(ApplicationFacade.SERVICE, () => new ServiceCommand());
            Facade.RegisterProxy(new ServiceProxy(() => new SqlConnection(connection)));
            Facade.RegisterMediator(new ServiceMediator((Service) notification.Body));
        }
    }
}