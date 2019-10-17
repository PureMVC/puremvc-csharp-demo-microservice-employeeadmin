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
using System.Linq;
using Employee.Model.ValueObject;
using PureMVC.Patterns.Proxy;

namespace Employee.Model
{
    public class ServiceProxy : Proxy
    {
        
        public ServiceProxy(Func<IDbConnection> connectionFactory) : base(NAME)
        {
            ConnectionFactory = connectionFactory;
        }

        public IList<EmployeeVO> FindAll()
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"
                    SELECT Employee.Id, Employee.Username, Employee.First, Employee.Last, Employee.Email,
                        STRING_AGG(Role.Id, ',') WITHIN GROUP(ORDER BY Role.Id) RoleIds,
                        STRING_AGG(Role.Name, ',') WITHIN GROUP(ORDER BY Role.Id) RoleNames,
                        Department.Id AS [Department.Id], Department.Name AS [Department.Name]
                    FROM Employee
                    INNER JOIN Department ON Employee.DepartmentID = department.id
                    LEFT JOIN EmployeeRole ON Employee.Id = EmployeeRole.EmployeeId
                    LEFT JOIN Role ON EmployeeRole.RoleId = Role.Id
                    GROUP BY Employee.Id, Employee.Username, Employee.First, Employee.Last, Employee.Email, Department.Id, Department.Name
                    ORDER BY Employee.Id";
                
                using (var reader = command.ExecuteReader())
                {
                    IList<EmployeeVO> rows = new List<EmployeeVO>();
                    while (reader.Read())
                    {
                        IList<RoleVO> roles = new List<RoleVO>();
                        if (!reader.IsDBNull(reader.GetOrdinal("RoleIds")))
                        {
                            var roleIds = reader.GetString(reader.GetOrdinal("RoleIds")).Split(",");
                            var roleNames = reader.GetString(reader.GetOrdinal("RoleNames")).Split(",");
                            roles = roleIds.Zip(roleNames, (roleId, roleName) => new RoleVO
                            {
                                Id = Int32.Parse(roleId),
                                Name = roleName
                            }).ToList();
                        }

                        rows.Add(new EmployeeVO
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")), 
                            Username = reader.GetString(reader.GetOrdinal("Username")), 
                            First = reader.GetString(reader.GetOrdinal("First")), 
                            Last = reader.GetString(reader.GetOrdinal("Last")), 
                            Email = reader.GetString(reader.GetOrdinal("Email")), 
                            Roles = roles,
                            Department = new DepartmentVO
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Department.Id")), 
                                Name = reader.GetString(reader.GetOrdinal("Department.Name"))
                            }
                        });
                    }
                    
                    return rows;
                }
            }
        }

        public EmployeeVO FindById(int id)
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"
                    SELECT Employee.Id, Employee.Username, Employee.First, Employee.Last, Employee.Email,
                        STRING_AGG(Role.Id, ',') WITHIN GROUP(ORDER BY Role.Id) RoleIds,
                        STRING_AGG(Role.Name, ',') WITHIN GROUP(ORDER BY Role.Id) RoleNames,
                        Department.Id AS [Department.Id], Department.Name AS [Department.Name]
                    FROM Employee
                    INNER JOIN Department ON Employee.DepartmentID = department.id
                    LEFT JOIN EmployeeRole ON Employee.Id = EmployeeRole.EmployeeId
                    LEFT JOIN Role ON EmployeeRole.RoleId = Role.Id
                    WHERE Employee.Id = @Id
                    GROUP BY Employee.Id, Employee.Username, Employee.First, Employee.Last, Employee.Email, Department.Id, Department.Name";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    IList<RoleVO> roles = new List<RoleVO>();
                    if (!reader.IsDBNull(reader.GetOrdinal("RoleIds")))
                    {
                        var roleIds = reader.GetString(reader.GetOrdinal("RoleIds")).Split(",");
                        var roleNames = reader.GetString(reader.GetOrdinal("RoleNames")).Split(",");
                        roles = roleIds.Zip(roleNames, (roleId, roleName) => new RoleVO
                        {
                            Id = Int32.Parse(roleId),
                            Name = roleName
                        }).ToList();
                    }
                    
                    return new EmployeeVO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")), 
                        Username = reader.GetString(reader.GetOrdinal("Username")), 
                        First = reader.GetString(reader.GetOrdinal("First")), 
                        Last = reader.GetString(reader.GetOrdinal("Last")), 
                        Email = reader.GetString(reader.GetOrdinal("Email")), 
                        Roles = roles,
                        Department = new DepartmentVO 
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Department.Id")),
                            Name = reader.GetString(reader.GetOrdinal("Department.Name"))
                        }
                    };
                }
            }
        }

        public int Save(EmployeeVO employee)
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"
                    INSERT INTO Employee(Username, First, Last, Email, DepartmentId) VALUES(@Username, @First, @Last, @Email, @DepartmentId);
                    SELECT SCOPE_IDENTITY()";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Username";
                parameter.Value = employee.Username;
                command.Parameters.Add(parameter);

                parameter = command.CreateParameter();
                parameter.ParameterName = "@First";
                parameter.Value = employee.First;
                command.Parameters.Add(parameter);
            
                parameter = command.CreateParameter();
                parameter.ParameterName = "@Last";
                parameter.Value = employee.Last;
                command.Parameters.Add(parameter);
            
                parameter = command.CreateParameter();
                parameter.ParameterName = "@Email";
                parameter.Value = employee.Email;
                command.Parameters.Add(parameter);
            
                parameter = command.CreateParameter();
                parameter.ParameterName = "@DepartmentId";
                parameter.Value = employee.Department.Id;
                command.Parameters.Add(parameter);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
        
        public int UpdateById(int id, EmployeeVO employee)
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"UPDATE EMPLOYEE SET First = @First, Last = @Last, Email = @Email, DepartmentId = @DepartmentId WHERE Id = @Id";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@First";
                parameter.Value = employee.First;
                command.Parameters.Add(parameter);
                
                parameter = command.CreateParameter();
                parameter.ParameterName = "@Last";
                parameter.Value = employee.Last;
                command.Parameters.Add(parameter);
                
                parameter = command.CreateParameter();
                parameter.ParameterName = "@Email";
                parameter.Value = employee.Email;
                command.Parameters.Add(parameter);
                
                parameter = command.CreateParameter();
                parameter.ParameterName = "@DepartmentId";
                parameter.Value = employee.Department.Id;
                command.Parameters.Add(parameter);
                
                parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                
                return command.ExecuteNonQuery();
            }
        }

        public int DeleteById(int id)
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"DELETE FROM Employee WHERE Id = @Id";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                
                return command.ExecuteNonQuery();
            }
        }
        
        private Func<IDbConnection> ConnectionFactory { get; }

        public new const string NAME = "ServiceProxy";
    }
}