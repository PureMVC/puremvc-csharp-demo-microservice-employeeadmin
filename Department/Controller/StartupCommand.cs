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
using Consul;
using Department.Model;
using Department.View;
using Department.View.Components;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace Department.Controller
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
                {"SERVICE_PORT", Environment.GetEnvironmentVariable("SERVICE_PORT")}
            };

            foreach (var (key, value) in env)
            {
                if(value == null) throw new SystemException($@"Please set the {key} in env variables and try again.");
            }
            
            using (var consul = new ConsulClient(configuration => configuration.Address = new Uri(env["CONSUL_HOST"])))
            {
                var registration = new AgentServiceRegistration
                {
                    ID = "department",
                    Name = "department",
                    Address = Dns.GetHostName(),
                    Port = Convert.ToInt32(env["SERVICE_PORT"])
                };

                consul.Agent.ServiceDeregister(registration.ID).Wait();
                consul.Agent.ServiceRegister(registration).Wait();
                Console.WriteLine("Registered with Consul.");
                
                var connection = $@"
                    Data Source={env["DATABASE_HOST"]};
                    Initial Catalog={env["DATABASE_NAME"]};
                    Persist Security Info=True;
                    User ID={env["DATABASE_USER"]};
                    Password={env["SA_PASSWORD"]};";
    
                Facade.RegisterCommand(ApplicationFacade.SERVICE, () => new ServiceCommand());
                Facade.RegisterProxy(new ServiceProxy(() => new SqlConnection(connection)));
                Facade.RegisterMediator(new ServiceMediator((Service) notification.Body));
            }
        }
    }
}