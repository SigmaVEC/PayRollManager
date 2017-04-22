
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/22/2017 12:01:31
-- Generated from EDMX file: C:\Users\Test\Source\Repos\PayRollManager\PayRollManager\Models\PayRollModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [PayRollManager];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Attendance_Details]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Attendance_Details];
GO
IF OBJECT_ID(N'[dbo].[Company_Info]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Company_Info];
GO
IF OBJECT_ID(N'[dbo].[Employee_Info]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employee_Info];
GO
IF OBJECT_ID(N'[dbo].[Employee_Salary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employee_Salary];
GO
IF OBJECT_ID(N'[dbo].[Payroll_History]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Payroll_History];
GO
IF OBJECT_ID(N'[dbo].[Personal_Details]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Personal_Details];
GO
IF OBJECT_ID(N'[dbo].[Salary_Bonus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Salary_Bonus];
GO
IF OBJECT_ID(N'[dbo].[Salary_Increments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Salary_Increments];
GO
IF OBJECT_ID(N'[dbo].[Salary_Slab]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Salary_Slab];
GO
IF OBJECT_ID(N'[dbo].[Session_Tokens]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Session_Tokens];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Attendance_Details'
CREATE TABLE [dbo].[Attendance_Details] (
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL,
    [Date] datetime  NOT NULL,
    [Shift] int  NOT NULL
);
GO

-- Creating table 'Company_Info'
CREATE TABLE [dbo].[Company_Info] (
    [CompanyId] int  NOT NULL,
    [CompanyName] varchar(max)  NOT NULL
);
GO

-- Creating table 'Employee_Info'
CREATE TABLE [dbo].[Employee_Info] (
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL,
    [EmployeeName] varchar(max)  NOT NULL,
    [DOL] datetime  NULL,
    [DOJ] datetime  NOT NULL,
    [IsAdmin] varchar(1)  NOT NULL,
    [Password] varchar(50)  NOT NULL
);
GO

-- Creating table 'Employee_Salary'
CREATE TABLE [dbo].[Employee_Salary] (
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL,
    [AdjustmentName] varchar(50)  NOT NULL,
    [AdjustmentValue] float  NOT NULL,
    [AdjustmentType] varchar(1)  NOT NULL
);
GO

-- Creating table 'Payroll_History'
CREATE TABLE [dbo].[Payroll_History] (
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL,
    [Date] datetime  NOT NULL,
    [AdjustmentName] varchar(50)  NOT NULL,
    [AdjustmentValue] float  NOT NULL,
    [AdjustmentType] varchar(1)  NOT NULL
);
GO

-- Creating table 'Personal_Details'
CREATE TABLE [dbo].[Personal_Details] (
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Value] varchar(max)  NOT NULL
);
GO

-- Creating table 'Salary_Bonus'
CREATE TABLE [dbo].[Salary_Bonus] (
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL,
    [ApplyDate] datetime  NOT NULL,
    [BonusName] varchar(50)  NOT NULL,
    [BonusType] varchar(1)  NOT NULL,
    [BonusValue] float  NOT NULL,
    [TargetAttendance] int  NOT NULL,
    [AvailableRepeats] int  NOT NULL
);
GO

-- Creating table 'Salary_Increments'
CREATE TABLE [dbo].[Salary_Increments] (
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL,
    [ApplyDate] datetime  NOT NULL,
    [IncrementName] varchar(50)  NOT NULL,
    [IncrementType] varchar(50)  NOT NULL,
    [IncrementValue] float  NOT NULL
);
GO

-- Creating table 'Salary_Slab'
CREATE TABLE [dbo].[Salary_Slab] (
    [CompanyId] int  NOT NULL,
    [FromAmount] float  NOT NULL,
    [ToAmount] float  NOT NULL,
    [Value] float  NOT NULL
);
GO

-- Creating table 'Session_Tokens'
CREATE TABLE [dbo].[Session_Tokens] (
    [SessionToken] varchar(70)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CompanyId] int  NOT NULL,
    [EmployeeId] varchar(50)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [CompanyId], [EmployeeId], [Date] in table 'Attendance_Details'
ALTER TABLE [dbo].[Attendance_Details]
ADD CONSTRAINT [PK_Attendance_Details]
    PRIMARY KEY CLUSTERED ([CompanyId], [EmployeeId], [Date] ASC);
GO

-- Creating primary key on [CompanyId] in table 'Company_Info'
ALTER TABLE [dbo].[Company_Info]
ADD CONSTRAINT [PK_Company_Info]
    PRIMARY KEY CLUSTERED ([CompanyId] ASC);
GO

-- Creating primary key on [CompanyId], [EmployeeId] in table 'Employee_Info'
ALTER TABLE [dbo].[Employee_Info]
ADD CONSTRAINT [PK_Employee_Info]
    PRIMARY KEY CLUSTERED ([CompanyId], [EmployeeId] ASC);
GO

-- Creating primary key on [CompanyId], [EmployeeId], [AdjustmentName] in table 'Employee_Salary'
ALTER TABLE [dbo].[Employee_Salary]
ADD CONSTRAINT [PK_Employee_Salary]
    PRIMARY KEY CLUSTERED ([CompanyId], [EmployeeId], [AdjustmentName] ASC);
GO

-- Creating primary key on [CompanyId], [EmployeeId], [Date], [AdjustmentName] in table 'Payroll_History'
ALTER TABLE [dbo].[Payroll_History]
ADD CONSTRAINT [PK_Payroll_History]
    PRIMARY KEY CLUSTERED ([CompanyId], [EmployeeId], [Date], [AdjustmentName] ASC);
GO

-- Creating primary key on [CompanyId], [EmployeeId], [Name] in table 'Personal_Details'
ALTER TABLE [dbo].[Personal_Details]
ADD CONSTRAINT [PK_Personal_Details]
    PRIMARY KEY CLUSTERED ([CompanyId], [EmployeeId], [Name] ASC);
GO

-- Creating primary key on [CompanyId], [EmployeeId], [ApplyDate], [BonusName] in table 'Salary_Bonus'
ALTER TABLE [dbo].[Salary_Bonus]
ADD CONSTRAINT [PK_Salary_Bonus]
    PRIMARY KEY CLUSTERED ([CompanyId], [EmployeeId], [ApplyDate], [BonusName] ASC);
GO

-- Creating primary key on [CompanyId], [EmployeeId], [ApplyDate], [IncrementName] in table 'Salary_Increments'
ALTER TABLE [dbo].[Salary_Increments]
ADD CONSTRAINT [PK_Salary_Increments]
    PRIMARY KEY CLUSTERED ([CompanyId], [EmployeeId], [ApplyDate], [IncrementName] ASC);
GO

-- Creating primary key on [CompanyId], [FromAmount], [ToAmount] in table 'Salary_Slab'
ALTER TABLE [dbo].[Salary_Slab]
ADD CONSTRAINT [PK_Salary_Slab]
    PRIMARY KEY CLUSTERED ([CompanyId], [FromAmount], [ToAmount] ASC);
GO

-- Creating primary key on [SessionToken] in table 'Session_Tokens'
ALTER TABLE [dbo].[Session_Tokens]
ADD CONSTRAINT [PK_Session_Tokens]
    PRIMARY KEY CLUSTERED ([SessionToken] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------