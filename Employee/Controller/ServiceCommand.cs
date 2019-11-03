//
//  ServiceCommand.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Text.RegularExpressions;
using Employee.Model;
using Employee.Model.Request;
using Employee.Model.ValueObject;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace Employee.Controller
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
                    case "/employees":
                        if (context.Request.Method == "GET")
                        {
                            serviceRequest.SetResultData(200, serviceProxy.FindAll());
                        } 
                        else if (context.Request.Method == "POST")
                        {
                            var resultData = serviceRequest.GetJson<EmployeeVO>();
                            resultData.Id = serviceProxy.Save(resultData);
                            serviceRequest.SetResultData(201, resultData);
                        }
                        else
                        {
                            serviceRequest.SetResultData(405, new {code = 405, message = "Method Not Allowed"});
                        }
                        break;

                    case "/health":
                        serviceRequest.SetResultData(200, null);
                        break;

                    default:
                        var match = Regex.Match(context.Request.Path.Value, @"/employees/(\d+)"); // employees/:id
                        if (match.Success)
                        {
                            if (context.Request.Method == "GET")
                            {
                                var employee = serviceProxy.FindById(Int32.Parse(match.Groups[1].Value));
                                if (employee != null)
                                {
                                    serviceRequest.SetResultData(200, employee);
                                }
                                else
                                {
                                    serviceRequest.SetResultData(404, new {code = 404, message = "Not Found"});
                                }
                            }
                            else if (context.Request.Method == "PUT")
                            {
                                if (serviceProxy.UpdateById(Int32.Parse(match.Groups[1].Value), serviceRequest.GetJson<EmployeeVO>()) == 1)
                                {
                                    var employee = serviceRequest.GetJson<EmployeeVO>();
                                    var result = new EmployeeVO {Id = Int32.Parse(match.Groups[1].Value), First = employee.First, Last = employee.Last, Email = employee.Email, Department = employee.Department};
                                    serviceRequest.SetResultData(200, result);
                                }
                                else
                                {
                                    serviceRequest.SetResultData(404, new {code = 404, message = "Not Found"});
                                }
                            }
                            else if (context.Request.Method == "DELETE")
                            {
                                if (serviceProxy.DeleteById(Int32.Parse(match.Groups[1].Value)) == 1)
                                {
                                    serviceRequest.SetResultData(204, null);
                                }
                                else
                                {
                                    serviceRequest.SetResultData(404, new {code = 404, message = "Not Found"});
                                }
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