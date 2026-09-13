CREATE DATABASE Pets;
GO
USE Pets;
GO

-- CUSTOMER TABLE
CREATE TABLE Customer (
    IdCustomer INT PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    City NVARCHAR(50) NOT NULL,
    State NVARCHAR(50) NOT NULL,
    Country NVARCHAR(200) NOT NULL,
    Address NVARCHAR(20) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    ContactPreference NVARCHAR(20) CHECK (ContactPreference IN ('Call', 'Whatsapp'))
);
GO

-- EMPLOYES TABLE
CREATE TABLE Employes (
    Id NVARCHAR(20) PRIMARY KEY,
    DateOfBirth DATE NOT NULL,
    StartDate DATE NOT NULL,
    SalaryPerDay DECIMAL(10,2) NOT NULL,
    RetirementDate DATE NULL,
    TypeEmploye NVARCHAR(20) CHECK (TypeEmploye IN ('Veterinary', 'Asistent', 'Administive', 'Manteinance', 'Groomer'))
);
GO

-- PETS TABLE
CREATE TABLE Pets (
    PetId INT IDENTITY(1,1) PRIMARY KEY,
    IdCustomer INT NOT NULL,
    PetName NVARCHAR(100) NOT NULL,
    AnimalSpecies NVARCHAR(20) CHECK (AnimalSpecies IN ('Horse', 'Dog', 'Cat', 'Fish', 'Goat', 'Rabbit', 'Cow', 'Pig', 'Rodent')),
    Breed NVARCHAR(50) NOT NULL,
    Age INT CHECK (Age BETWEEN 0 AND 100),
    Color NVARCHAR(50) NOT NULL,
    LastServiceDate DATE NULL,
    PhoneOwner NVARCHAR(15),
    EmailOwner NVARCHAR(100),
    CONSTRAINT FK_Pets_Customers FOREIGN KEY (IdCustomer) REFERENCES Customer(IdCustomer)
);
GO

-- PROCEDURES TABLE
CREATE TABLE Procedures (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdCustomer INT NOT NULL,
    PetId INT NOT NULL,
    ProcedureType NVARCHAR(50) NOT NULL,
    Weight FLOAT NULL,
    BasePrice DECIMAL(10,2) NOT NULL,
    VAT AS (BasePrice * 0.13) PERSISTED,
    TotalPrice AS (BasePrice + (BasePrice * 0.13)) PERSISTED,
    Status NVARCHAR(20) NOT NULL,
    CONSTRAINT FK_Procedures_Customers FOREIGN KEY (IdCustomer) REFERENCES Customer(IdCustomer),
    CONSTRAINT FK_Procedures_Pets FOREIGN KEY (PetId) REFERENCES Pets(PetId)
);
GO

-- REPORTS TABLE
CREATE TABLE Reports (
    IdReport INT PRIMARY KEY IDENTITY(1,1),
    Id INT NOT NULL, -- Related procedure Id
    IdCustomer INT NOT NULL,
    PetId INT NOT NULL,
    Name NVARCHAR(150) NULL,
    PetName NVARCHAR(100) NULL,
    ProcedureType NVARCHAR(100) NULL,
    ProcedureStatus NVARCHAR(50) NULL,
    Weight FLOAT NULL,
    BasePrice DECIMAL(10,2) NULL,
    VAT DECIMAL(10,2) NULL,
    TotalPrice DECIMAL(10,2) NULL,
    VaccinationDate DATE NULL,
    CONSTRAINT FK_Reports_Customers FOREIGN KEY (IdCustomer) REFERENCES Customer(IdCustomer),
    CONSTRAINT FK_Reports_Pets FOREIGN KEY (PetId) REFERENCES Pets(PetId),
    CONSTRAINT FK_Reports_Procedures FOREIGN KEY (Id) REFERENCES Procedures(Id)
);
GO
