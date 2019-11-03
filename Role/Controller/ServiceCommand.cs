//
//  ServiceCommand.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Role.Model;
using Role.Model.Request;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace Role.Controller
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
                    case "/health":
                        serviceRequest.SetResultData(200, null);
                        break;

                    default:
                        var match = Regex.Match(context.Request.Path.Value, @"/employees/(\d+)/roles"); // employees/:id/roles
                        if (match.Success)
                        {
                            if (context.Request.Method == "GET")
                            {
                                var roles = serviceProxy.GetUserRolesById(Int32.Parse(match.Groups[1].Value));
                                if (roles != null)
                                {
                                    serviceRequest.SetResultData(200, roles);
                                }
                                else
                                {
                                    serviceRequest.SetResultData(404, new {code = 404, message = "Not Found"});
                                }
                            }
                            else if (context.Request.Method == "PUT")
                            {
                                var _ = serviceProxy.UpdateUserRolesById(Int32.Parse(match.Groups[1].Value), serviceRequest.GetJson<List<int>>());
                                
                                serviceRequest.SetResultData(200, serviceRequest.GetJson<List<int>>());
                            }
                            else
                            {
                                serviceRequest.SetResultData(405, new {code = 405, message = "Method Not Allowed"});
                            }
                        }
                        else
                        {
                            serviceRequest.SetResultData(404, new {code = 404, message = "Not Found"});
                        }
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