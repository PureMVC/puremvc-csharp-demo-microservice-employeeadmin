//
//  EmployeeVO.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Collections.Generic;

namespace Employee.Model.ValueObject
{
    public class EmployeeVO
    {
        public int Id { get; set;  }

        public string Username { get; set; }
        
        public string First { get; set; }
        
        public string Last { get; set; }
        
        public string Email { get; set; }

        public IList<RoleVO> Roles { get; set; }
        
        public DepartmentVO Department { get; set; }
    }
}