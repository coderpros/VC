CREATE TABLE [dbo].[tblCollection]
(
	[tblCollection_id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[tblCollection_name] NVARCHAR(200) NOT NULL,
    [tblCollection_desc] NVARCHAR(MAX) NULL, 
    [tblCollection_enabled] bit NOT NULL DEFAULT 0,
    [tblCollection_saveToUmbraco] bit NOT NULL DEFAULT 0,
    [tblCollection_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblCollection_createdBy] NVARCHAR(200) NULL, 
    [tblCollection_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblCollection_editedBy] NVARCHAR(200) NULL, 
    [tblCollection_deletedUTC] DATETIME NULL, 
    [tblCollection_deletedBy] NVARCHAR(200) NULL
)
