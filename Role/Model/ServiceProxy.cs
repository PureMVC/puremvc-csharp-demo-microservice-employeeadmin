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
using Role.Model.ValueObject;
using PureMVC.Patterns.Proxy;

namespace Role.Model
{
    public class ServiceProxy : Proxy
    {
        
        public ServiceProxy(Func<IDbConnection> connectionFactory) : base(NAME)
        {
            ConnectionFactory = connectionFactory;
        }

        public IList<RoleVO> GetUserRolesById(int id)
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"
                    SELECT First FROM Employee WHERE Id = @Id;
                    SELECT Role.Id AS [Role.Id], Role.NAME AS [Role.Name]
                     FROM Employee
                     INNER JOIN EmployeeRole on Employee.Id = EmployeeRole.EmployeeId
                     INNER JOIN Role on EmployeeRole.RoleId = Role.Id
                    WHERE Employee.Id = @Id";
                    
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    if (!reader.NextResult()) return null;
                    IList<RoleVO> roles = new List<RoleVO>();
                    while (reader.Read())
                    {
                        roles.Add(new RoleVO 
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Role.Id")), 
                            Name = reader.GetString(reader.GetOrdinal("Role.Name"))
                        });
                    }
                    return roles;
                }
            }
        }
        
        public int UpdateUserRolesById(int id, List<int> roleIds)
        {
            using (var connection = ConnectionFactory.Invoke())
            using (var command = connection.CreateCommand())
            {
                connection.Open();
            
                var values = roleIds.Aggregate("", (accumulator, roleId) => accumulator + "(" + id + ", " + roleId + "),").TrimEnd(',');
                command.CommandText = $@"
                    SET XACT_ABORT ON;
                    BEGIN TRANSACTION
                        BEGIN TRY
                            DELETE FROM EmployeeRole WHERE EmployeeId = @Id;
                            INSERT INTO EmployeeRole(EmployeeId, RoleId) VALUES {values};
                            COMMIT TRANSACTION;
                        END TRY
                        BEGIN CATCH
                            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION; 
                            THROW;
                        END CATCH";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
        
        private Func<IDbConnection> ConnectionFactory { get; }
        
        public new const string NAME = "ServiceProxy";
    }
}