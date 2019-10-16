//
//  ApplicationFacade.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Department.Controller;
using Department.View.Components;
using PureMVC.Patterns.Facade;

namespace Department
{
    public class ApplicationFacade : Facade
    {
        private ApplicationFacade(string key) : base(key)
        {
        }
        
        protected override void InitializeController()
        {
            base.InitializeController();
            RegisterCommand(STARTUP, () => new StartupCommand());
        }

        public static ApplicationFacade GetInstance()
        {
            return (ApplicationFacade) Facade.GetInstance("service", () => new ApplicationFacade("service"));
        }

        public void Startup(Service startup)
        {
            SendNotification(STARTUP, startup);
        }

        public const string STARTUP = "startup";
        
        public const string SERVICE = "service";

        public const string SERVICE_RESULT = "serviceResult";

        public const string SERVICE_FAULT = "serviceFault";
    }
}