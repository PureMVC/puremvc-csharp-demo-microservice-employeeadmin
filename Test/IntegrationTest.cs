//
//  IntegrationTest.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Consul;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Test
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void TestFindAgent()
        {
            using (var consul = new ConsulClient(configuration => configuration.Address = new Uri("http://localhost:8500")))
            {
                Assert.AreEqual(3, consul.Agent.Services().Result.Response.Count);
                foreach (var (_, value) in consul.Agent.Services().Result.Response)
                {
                    Assert.IsNotNull(value.Address);
                }
            }
        }

        [TestMethod]
        public void TestFindAllEmployee()
        {
            var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees");
            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(JsonConvert.DeserializeObject<List<Employee.Model.ValueObject.EmployeeVO>>(reader.ReadToEnd()));
            }
        }

        [TestMethod]
        public void TestFindByIdInsertUpdateDeleteEmployee()
        {
            var employee = new Employee.Model.ValueObject.EmployeeVO
            {
                Username = "sshemp",
                First = "Shemp",
                Last = "Stooge",
                Email = "sshemp@stooges.com",
                Department = new Employee.Model.ValueObject.DepartmentVO
                {
                    Id = 2,
                    Name = "Sales"
                }
            };
            
            {   // save
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(employee));
                var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse) request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                    var data = JsonConvert.DeserializeObject<Employee.Model.ValueObject.EmployeeVO>(reader.ReadToEnd());
                    Assert.AreEqual("sshemp", data.Username);
                    Assert.AreEqual("Shemp", data.First);
                    Assert.AreEqual("Stooge", data.Last);
                    Assert.IsNotNull(data.Department);
                    Assert.AreEqual(2, data.Department.Id);
                    Assert.AreEqual("Sales", data.Department.Name);
                    employee.Id = data.Id;
                }
            }

            {   // update
                employee.First = "Joe";
                employee.Email = "joe@stooges.com";

                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(employee));
                var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees/" + employee.Id);
                request.Method = "PUT";
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse) request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var result = JsonConvert.DeserializeObject<Employee.Model.ValueObject.EmployeeVO>(reader.ReadToEnd());
                    Assert.AreEqual(employee.First, result.First);
                    Assert.AreEqual(employee.Email, result.Email);
                }
            }
            
            {   // select
                var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees/" + employee.Id);
                using (var response = (HttpWebResponse) request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var result = JsonConvert.DeserializeObject<Employee.Model.ValueObject.EmployeeVO>(reader.ReadToEnd());
                    Assert.AreEqual(employee.First, result.First);
                    Assert.AreEqual(employee.Email, result.Email);
                }
            }
            
            {   // delete
                var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees/" + employee.Id);
                request.Method = "DELETE";
                using (request.GetResponse())
                {
                }
            }
        }

        [TestMethod]
        public void TestFindAllRoles()
        {
            var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees/1/roles");
            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(JsonConvert.DeserializeObject<List<Role.Model.ValueObject.RoleVO>>(reader.ReadToEnd()));
            }
        }

        [TestMethod]
        public void TestFindByIdUpdateRoles()
        {
            int[] roleIds = {7, 8, 9};

            {   // update
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(roleIds));
                var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees/1/roles");
                request.Method = "PUT";
                request.ContentType = "application/json";
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            
                using (var response = (HttpWebResponse) request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var result = JsonConvert.DeserializeObject<int[]>(reader.ReadToEnd());
                    Assert.IsNotNull(result);
                    Assert.IsTrue(roleIds.SequenceEqual(result));
                }
            }

            {   // find
                var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/employees/1/roles");
                using (var response = (HttpWebResponse) request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var result = JsonConvert.DeserializeObject<List<Role.Model.ValueObject.RoleVO>>(reader.ReadToEnd());
                    Assert.IsTrue(roleIds.Length == result.Count);
                    result.ForEach(role =>
                        {
                            Assert.IsNotNull(role.Id);
                            Assert.IsNotNull(role.Name);
                        }
                    );
                }
            }
        }

        [TestMethod]
        public void TestFindAllDepartment()
        {
            var request = (HttpWebRequest) WebRequest.Create("http://localhost:6001/departments");
            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(JsonConvert.DeserializeObject<List<Department.Model.ValueObject.DepartmentVO>>(reader.ReadToEnd()));
            }
        }
    }
}