//
//  EmployeeVO.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Employee.Model.ValueObject
{
    public class EmployeeVO
    {
        [JsonPropertyName("id")]
        public int Id { get; set;  }

        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("first")]
        public string First { get; set; }
        
        [JsonPropertyName("last")]
        public string Last { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("roles")]
        public IList<RoleVO> Roles { get; set; }
        
        [JsonPropertyName("department")]
        public DepartmentVO Department { get; set; }
    }
}