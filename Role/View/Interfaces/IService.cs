//
//  IService.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Role.View.Interfaces
{
    public interface IService
    {
        Task<object> Service(HttpContext context);
    }
}