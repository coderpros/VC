CREATE TABLE [dbo].[tblCountry]
(
	[tblCountry_id] INT NOT NULL  IDENTITY,
    [tblCountry_websiteId] INT NULL,
    [tblCountry_name] NVARCHAR(200) NOT NULL, 
    [tblCountry_A2] NCHAR(2) NULL, 
    [tblCountry_A3] NCHAR(3) NULL, 
    [tblCountry_number] INT NULL, 
    [tblCountry_order] INT NOT NULL, 
    [tblCountry_enabled] BIT NOT NULL DEFAULT 0, 
    [tblCountry_taxRate] DECIMAL(8, 4) NULL, 
    [tblCountry_createdUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblCountry_createdBy] NVARCHAR(100) NULL, 
    [tblCountry_editedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblCountry_editedBy] NVARCHAR(100) NULL, 
    [tblCountry_deletedUtc] DATETIME NULL, 
    [tblCountry_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblCountry] PRIMARY KEY ([tblCountry_id])
)

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblCountry] ON [dbo].[tblCountry] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblCountry'
    END
