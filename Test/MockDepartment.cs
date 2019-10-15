//
//  MockDepartment.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Data;
using Department.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Test
{
    [TestClass]
    public class MockDepartment
    {
        [TestMethod]
        public void TestFindAll()
        {
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(_ => _.Read()).Returns(true).Returns(false);
            readerMock.Setup(reader => reader.GetOrdinal("Id")).Returns(0);
            readerMock.Setup(reader => reader.GetOrdinal("Name")).Returns(2);
            readerMock.Setup(reader => reader.GetInt32(It.IsAny<int>())).Returns(1);
            readerMock.Setup(reader => reader.GetString(It.IsAny<int>())).Returns("Accounting");
            
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();
            
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            
            var sut = new ServiceProxy(() => connectionMock.Object);
            var result = sut.FindAll();
            
            Assert.AreEqual(1, result.Count);
            commandMock.Verify();
        }
    }
}