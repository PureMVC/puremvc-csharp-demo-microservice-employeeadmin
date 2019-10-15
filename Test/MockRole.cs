//
//  MockRole.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Role.Model;

namespace Test
{
    [TestClass]
    public class MockRole
    {
        [TestMethod]
        public void TestGetUserRolesById()
        {
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(_ => _.Read()).Returns(true).Returns(true).Returns(false);
            readerMock.Setup(_ => _.NextResult()).Returns(true);
            readerMock.Setup(reader => reader.GetOrdinal("Role.Id")).Returns(0);
            readerMock.Setup(reader => reader.GetOrdinal("Role.Name")).Returns(2);
            readerMock.Setup(reader => reader.GetInt32(It.IsAny<int>())).Returns(1);
            readerMock.Setup(reader => reader.GetString(It.IsAny<int>())).Returns("Administrator");
            
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();
            
            var parameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(m => m.CreateParameter()).Returns(parameterMock.Object);
            commandMock.Setup(m => m.Parameters.Add(It.IsAny<IDbDataParameter>())).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var result = sut.GetUserRolesById(1);
            
            Assert.AreEqual(1, result.Count);
            commandMock.Verify();
        }

        [TestMethod]
        public void TestUpdateUserRolesById()
        {
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(_ => _.Read()).Returns(true).Returns(false);
            readerMock.Setup(reader => reader.GetOrdinal("Role.Id")).Returns(0);
            readerMock.Setup(reader => reader.GetOrdinal("Role.Name")).Returns(2);
            readerMock.Setup(reader => reader.GetInt32(It.IsAny<int>())).Returns(1);
            readerMock.Setup(reader => reader.GetString(It.IsAny<int>())).Returns("Administrator");
            
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteScalar()).Returns(1).Verifiable();

            var parameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(m => m.CreateParameter()).Returns(parameterMock.Object);
            commandMock.Setup(m => m.Parameters.Add(It.IsAny<IDbDataParameter>())).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var result = sut.UpdateUserRolesById(1, new List<int>{1, 2, 3});
            
            Assert.AreEqual(1, result);
            commandMock.Verify();
        }
    }
}