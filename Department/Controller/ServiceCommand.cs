//
//  ServiceCommand.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using Department.Model;
using Department.Model.Request;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace Department.Controller
{
    public class ServiceCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            var serviceRequest = (ServiceRequest) notification.Body;
            var serviceProxy = (ServiceProxy) Facade.RetrieveProxy(ServiceProxy.NAME);
            var context = serviceRequest.Context;
            
            try
            {
                switch (context.Request.Path.Value)
                {
                    case "/departments":
                        if (context.Request.Method == "GET")
                        {
                            serviceRequest.SetResultData(200, serviceProxy.FindAll());
                        }
                        else
                        {
                            serviceRequest.SetResultData(405, new {Code = 405, Message = "Method Not Allowed"});
                        }
                        break;

                    case "/health":
                        serviceRequest.SetResultData(200, null);
                        break;
                    
                    default:
                        serviceRequest.SetResultData(404, new {Code = 404, Message = "Not Found"});
                        break;
                }
            }
            catch (Exception exception)
            {
                serviceRequest.SetResultData(500, exception);
                SendNotification(ApplicationFacade.SERVICE_FAULT, serviceRequest);
                return;
            }

            SendNotification(serviceRequest.Status >= 200 && serviceRequest.Status < 299 ? ApplicationFacade.SERVICE_RESULT : ApplicationFacade.SERVICE_FAULT, serviceRequest);
        }
    }
}