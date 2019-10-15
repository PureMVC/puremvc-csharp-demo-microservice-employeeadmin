//
//  ServiceProxy.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Collections.Generic;
using System.Data;
using Department.Model.ValueObject;
using PureMVC.Patterns.Proxy;

namespace Department.Model
{
    public class ServiceProxy : Proxy
    {
        
        public ServiceProxy(Func<IDbConnection> connectionFactory) : base(NAME)
        {
            ConnectionFactory = connectionFactory;
        }

        public IList<DepartmentVO> FindAll()
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "SELECT Id, Name FROM Department";
                using (var reader = command.ExecuteReader())
                {
                    IList<DepartmentVO> rows = new List<DepartmentVO>();
                    while (reader.Read())
                    {
                        rows.Add(new DepartmentVO
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    return rows;
                }
            }
            
        }
        
        private Func<IDbConnection> ConnectionFactory { get; }

        public new const string NAME = "ServiceProxy";
    }
}