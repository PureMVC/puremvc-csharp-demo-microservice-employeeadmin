CREATE DATABASE EmployeeAdmin;
GO

USE EmployeeAdmin;
GO

--
-- Table structure for table Department
--

CREATE TABLE Department(
    Id INT IDENTITY CONSTRAINT Department_pk PRIMARY KEY NONCLUSTERED,
    Name VARCHAR (50) NOT NULL
);
GO

--
-- Dumping data for table Department
--

INSERT INTO Department(Name) VALUES ('Accounting'),('Sales'),('Plant'),('Shipping'),('Quality Control');
GO

--
-- Table structure for table Employee
--

CREATE TABLE Employee (
    Id INT IDENTITY CONSTRAINT Employee_pk PRIMARY KEY NONCLUSTERED,
    Username VARCHAR(50) NOT NULL,
    First VARCHAR(50) NOT NULL,
    Last VARCHAR(50) NOT NULL,
    Email VARCHAR(50) NOT NULL,
    DepartmentID INT NOT NULL
    CONSTRAINT Employee_Department_Id_fk REFERENCES Department ON UPDATE CASCADE ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX Employee_Username_uindex ON Employee (Username);
GO

--
-- Dumping data for table Employee
--

SET IDENTITY_INSERT Employee ON
GO
INSERT INTO Employee(Id, Username, First, Last, Email, DepartmentID) VALUES (1,'lstooge','Larry','Stooge','larry@stooges.com',1),(2,'cstooge','Curly','Stooge','curly@stooges.com',2),(3,'mstooge','Moe','Stooge','moe@stooges.com',3);
GO
SET IDENTITY_INSERT Employee OFF
GO

--
-- Table structure for table Role
--

CREATE TABLE Role (
    Id INT IDENTITY CONSTRAINT Role_pk PRIMARY KEY NONCLUSTERED,
    Name VARCHAR(50) NOT NULL
);
GO

--
-- Dumping data for table Role
--

SET IDENTITY_INSERT Role ON
GO
INSERT INTO Role(Id, Name) VALUES (1,'Administrator'),(2,'Accounts Payable'),(3,'Accounts Receivable'),(4,'Employee Benefits'),(5,'General Ledger'),(6,'Payroll'),(7,'Inventory'),(8,'Production'),(9,'Quality Control'),(10,'Sales'),(11,'Orders'),(12,'Customers'),(13,'Shipping'),(14,'Returns');
GO
SET IDENTITY_INSERT Role OFF
GO

--
-- Table structure for table EmployeeRole
--

CREATE TABLE EmployeeRole (
    EmployeeId INT NOT NULL CONSTRAINT EmployeeRole_Employee_Id_fk references Employee ON UPDATE CASCADE ON DELETE CASCADE,
    RoleId INT NOT NULL CONSTRAINT EmployeeRole_Role_Id_fk references Role ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT EmployeeRole_pk PRIMARY KEY NONCLUSTERED (EmployeeId, RoleId)
);
GO

--
-- Dumping data for table EmployeeRole
--

INSERT INTO EmployeeRole(EmployeeId, RoleId) VALUES (1,4),(1,6),(2,2),(2,3),(2,5),(3,7),(3,8),(3,9);
GO