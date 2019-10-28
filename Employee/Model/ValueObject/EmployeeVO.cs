//
//  EmployeeVO.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Employee.Model.ValueObject
{
    public class EmployeeVO
    {
        [JsonProperty("id")]
        public int Id { get; set;  }

        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("first")]
        public string First { get; set; }
        
        [JsonProperty("last")]
        public string Last { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("roles")]
        public IList<RoleVO> Roles { get; set; }
        
        [JsonProperty("department")]
        public DepartmentVO Department { get; set; }
    }
}