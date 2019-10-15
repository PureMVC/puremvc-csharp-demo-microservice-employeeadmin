//
//  ServiceMediator.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Threading.Tasks;
using Role.Model.Request;
using Role.View.Components;
using Role.View.Interfaces;
using Microsoft.AspNetCore.Http;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;

namespace Role.View
{
    public class ServiceMediator : Mediator, IService
    {
        public ServiceMediator(Service viewComponent) : base(NAME, viewComponent)
        {
        }

        public override void OnRegister()
        {
            ((Service) ViewComponent).Delegate = this;
        }
        
        public Task<object> Service(HttpContext context)
        {
            var request = new ServiceRequest(context);
            SendNotification(ApplicationFacade.SERVICE, request);
            return request.TaskCompletionSource.Task;
        }

        public override string[] ListNotificationInterests()
        {
            return new []
            {
                ApplicationFacade.SERVICE_RESULT,
                ApplicationFacade.SERVICE_FAULT
            };
        }

        public override void HandleNotification(INotification notification)
        {
            ServiceRequest request = (ServiceRequest) notification.Body;
            switch (notification.Name)
            {
                case ApplicationFacade.SERVICE_RESULT:
                    ((Service) ViewComponent).Result(request.Context, request.Status, request.ResultData);
                    request.TaskCompletionSource.SetResult(null);
                    break;
                
                case ApplicationFacade.SERVICE_FAULT:
                    ((Service) ViewComponent).Fault(request.Context, request.Status, request.ResultData);
                    request.TaskCompletionSource.SetResult(null);
                    break;
            }
        }

        public new const string NAME = "ServiceMediator";

    }
}