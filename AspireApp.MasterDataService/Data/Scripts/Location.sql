USE MasterDataDb;
GO

IF OBJECT_ID('dbo.Locations', 'U') IS NOT NULL
DROP TABLE dbo.Locations;
GO

CREATE TABLE Locations (
                           Id INT PRIMARY KEY IDENTITY(1,1),
                           Name NVARCHAR(100) NOT NULL,
                           Type NVARCHAR(50) NOT NULL,
                           ParentId INT NULL
);
GO

INSERT INTO Locations (Name, Type, ParentId) VALUES
('Main Site', 'Site', NULL),
('Room 101', 'Room', 1),
('Room 102', 'Room', 1),
('Storage A', 'Storage', 1),
('Sub Room A1', 'Room', 2),
('Sub Room A2', 'Room', 2);
GO
