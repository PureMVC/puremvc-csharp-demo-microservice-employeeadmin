//
//  MockEmployee.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Data;
using Employee.Model;
using Employee.Model.ValueObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Test
{
    [TestClass]
    public class MockEmployee
    {
        [TestMethod]
        public void TestFindAll()
        {
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(_ => _.Read()).Returns(true).Returns(false);
            readerMock.Setup(reader => reader.GetOrdinal("Id")).Returns(0);
            readerMock.Setup(reader => reader.GetInt32(0)).Returns(1);
            readerMock.Setup(reader => reader.GetOrdinal("Username")).Returns(1);
            readerMock.Setup(reader => reader.GetString(1)).Returns("lstooge");
            readerMock.Setup(reader => reader.GetOrdinal("First")).Returns(2);
            readerMock.Setup(reader => reader.GetString(2)).Returns("Larry");
            readerMock.Setup(reader => reader.GetOrdinal("Last")).Returns(3);
            readerMock.Setup(reader => reader.GetString(3)).Returns("Stooge");
            readerMock.Setup(reader => reader.GetOrdinal("Email")).Returns(4);
            readerMock.Setup(reader => reader.GetString(4)).Returns("larry@stooges.com");
            
            readerMock.Setup(reader => reader.GetOrdinal("RoleIds")).Returns(5);
            readerMock.Setup(reader => reader.GetString(5)).Returns("1,11");
            readerMock.Setup(reader => reader.GetOrdinal("RoleNames")).Returns(6);
            readerMock.Setup(reader => reader.GetString(6)).Returns("Administrator,Orders");
            
            readerMock.Setup(reader => reader.GetOrdinal("Department.Id")).Returns(7);
            readerMock.Setup(reader => reader.GetInt32(7)).Returns(1);
            readerMock.Setup(reader => reader.GetOrdinal("Department.Name")).Returns(8);
            readerMock.Setup(reader => reader.GetString(8)).Returns("Accounting");
            
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var result = sut.FindAll();
            Assert.AreEqual(1, result.Count);
            
            var employee = result[0];
            Assert.AreEqual(1, employee.Id);
            Assert.AreEqual("lstooge", employee.Username);
            Assert.AreEqual("Larry", employee.First);
            Assert.AreEqual("Stooge", employee.Last);
            Assert.AreEqual("larry@stooges.com", employee.Email);
            
            Assert.IsNotNull(employee.Roles);
            Assert.AreEqual(2, employee.Roles.Count);
            Assert.AreEqual(1, employee.Roles[0].Id);
            Assert.AreEqual("Administrator", employee.Roles[0].Name);
            Assert.AreEqual(11, employee.Roles[1].Id);
            Assert.AreEqual("Orders", employee.Roles[1].Name);
            
            Assert.IsNotNull(employee.Department);
            Assert.AreEqual(1, employee.Department.Id);
            Assert.AreEqual("Accounting", employee.Department.Name);
            
            commandMock.Verify();
        }

        [TestMethod]
        public void TestFindEmployeeById()
        {
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(_ => _.Read()).Returns(true).Returns(false);
            readerMock.Setup(reader => reader.GetOrdinal("Id")).Returns(0);
            readerMock.Setup(reader => reader.GetInt32(0)).Returns(1);
            readerMock.Setup(reader => reader.GetOrdinal("Username")).Returns(1);
            readerMock.Setup(reader => reader.GetString(1)).Returns("lstooge");
            readerMock.Setup(reader => reader.GetOrdinal("First")).Returns(2);
            readerMock.Setup(reader => reader.GetString(2)).Returns("Larry");
            readerMock.Setup(reader => reader.GetOrdinal("Last")).Returns(3);
            readerMock.Setup(reader => reader.GetString(3)).Returns("Stooge");
            readerMock.Setup(reader => reader.GetOrdinal("Email")).Returns(4);
            readerMock.Setup(reader => reader.GetString(4)).Returns("larry@stooges.com");
            
            readerMock.Setup(reader => reader.GetOrdinal("RoleIds")).Returns(5);
            readerMock.Setup(reader => reader.GetString(5)).Returns("1,11");
            readerMock.Setup(reader => reader.GetOrdinal("RoleNames")).Returns(6);
            readerMock.Setup(reader => reader.GetString(6)).Returns("Administrator,Orders");
            
            readerMock.Setup(reader => reader.GetOrdinal("Department.Id")).Returns(7);
            readerMock.Setup(reader => reader.GetInt32(7)).Returns(1);
            readerMock.Setup(reader => reader.GetOrdinal("Department.Name")).Returns(8);
            readerMock.Setup(reader => reader.GetString(8)).Returns("Accounting");
            
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();
            
            var parameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(m => m.CreateParameter()).Returns(parameterMock.Object);
            commandMock.Setup(m => m.Parameters.Add(It.IsAny<IDbDataParameter>())).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var employee = sut.FindById(1);
            Assert.IsNotNull(employee);
            
            Assert.AreEqual(1, employee.Id);
            Assert.AreEqual("lstooge", employee.Username);
            Assert.AreEqual("Larry", employee.First);
            Assert.AreEqual("Stooge", employee.Last);
            Assert.AreEqual("larry@stooges.com", employee.Email);
            
            Assert.IsNotNull(employee.Roles);
            Assert.AreEqual(2, employee.Roles.Count);
            Assert.AreEqual(1, employee.Roles[0].Id);
            Assert.AreEqual("Administrator", employee.Roles[0].Name);
            Assert.AreEqual(11, employee.Roles[1].Id);
            Assert.AreEqual("Orders", employee.Roles[1].Name);
            
            Assert.IsNotNull(employee.Department);
            Assert.AreEqual(1, employee.Department.Id);
            Assert.AreEqual("Accounting", employee.Department.Name);
            
            commandMock.Verify();
        }

        [TestMethod]
        public void TestSave()
        {
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteScalar()).Returns(4).Verifiable();
            
            var parameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(m => m.CreateParameter()).Returns(parameterMock.Object);
            commandMock.Setup(m => m.Parameters.Add(It.IsAny<IDbDataParameter>())).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var result = sut.Save(new EmployeeVO
            {
                Username = "sshemp",
                First = "Shemp",
                Last = "Stooge",
                Email = "sshemp@stooges.com",
                Department = new DepartmentVO
                {
                    Id = 2,
                    Name = "Sales"
                }
            });
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void TestUpdateById()
        {
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteNonQuery()).Returns(1).Verifiable();
            
            var parameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(m => m.CreateParameter()).Returns(parameterMock.Object);
            commandMock.Setup(m => m.Parameters.Add(It.IsAny<IDbDataParameter>())).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var result = sut.UpdateById(4, new EmployeeVO
            {
                Username = "sshemp",
                First = "Shemp",
                Last = "Stooge",
                Email = "sshemp@stooges.com",
                Department = new DepartmentVO
                {
                    Id = 2,
                    Name = "Sales"
                }
            });
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDelete()
        {
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteNonQuery()).Returns(1).Verifiable();
            
            var parameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(m => m.CreateParameter()).Returns(parameterMock.Object);
            commandMock.Setup(m => m.Parameters.Add(It.IsAny<IDbDataParameter>())).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var result = sut.DeleteById(4);
            Assert.AreEqual(1, result);
        }
    }
}