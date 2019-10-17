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
using System.Net;
using System.Threading.Tasks;
using Consul;
using Role.Model;
using Role.View;
using Role.View.Components;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace Role.Controller
{
    public class StartupCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            var env = new Dictionary<string, string>
            {
                {"DATABASE_HOST", Environment.GetEnvironmentVariable("DATABASE_HOST")},
                {"DATABASE_NAME", Environment.GetEnvironmentVariable("DATABASE_NAME")},
                {"DATABASE_USER", Environment.GetEnvironmentVariable("DATABASE_USER")},
                {"SA_PASSWORD", Environment.GetEnvironmentVariable("SA_PASSWORD")},
                {"CONSUL_HOST", Environment.GetEnvironmentVariable("CONSUL_HOST")},
                {"APPLICATION_NAME", Environment.GetEnvironmentVariable("APPLICATION_NAME")},
                {"SERVICE_PORT", Environment.GetEnvironmentVariable("SERVICE_PORT")}
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
            do
            {
                try
                {
                    using (var sqlConnection = new SqlConnection(connection))
                    {
                        sqlConnection.Open();
                        Console.WriteLine("Connected to the Database.");
                        break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Task.Delay(5000).Wait();
                }
            } while (true);
            
            do
            {
                try
                {
                    using (var consul = new ConsulClient(configuration => configuration.Address = new Uri(env["CONSUL_HOST"])))
                    {
                        var registration = new AgentServiceRegistration
                        {
                            ID = env["APPLICATION_NAME"],
                            Name = env["APPLICATION_NAME"],
                            Address = Dns.GetHostName(),
                            Port = Convert.ToInt32(env["SERVICE_PORT"])
                        };
                        consul.Agent.ServiceDeregister(registration.ID).Wait();
                        consul.Agent.ServiceRegister(registration).Wait();
                        Console.WriteLine("Registered with the Consul.");
                        break;
                    }
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