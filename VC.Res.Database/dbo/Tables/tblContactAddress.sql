CREATE TABLE [dbo].[tblContactAddress]
(
	[tblContactAddress_id] INT NOT NULL  IDENTITY, 
    [tblContact_id] INT NOT NULL,
    [tblContactAddress_line1] NVARCHAR(100) NOT NULL, 
    [tblContactAddress_line2] NVARCHAR(100) NULL, 
    [tblContactAddress_line3] NVARCHAR(100) NULL, 
    [tblContactAddress_town] NVARCHAR(100) NOT NULL, 
    [tblContactAddress_region] NVARCHAR(100) NULL, 
    [tblContactAddress_postCode] NVARCHAR(30) NULL, 
    [tblCountry_id] INT NOT NULL,
    [tblContactAddress_primary] BIT NOT NULL DEFAULT 0, 
    [tblContactAddress_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblContactAddress_createdBy] NVARCHAR(100) NULL, 
    [tblContactAddress_editedUTC] DATETIME NOT NULL, 
    [tblContactAddress_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblContactAddress] PRIMARY KEY ([tblContactAddress_id]), 
    CONSTRAINT [FK_tblContactAddress_tblContact] FOREIGN KEY ([tblContact_id]) REFERENCES [tblContact]([tblContact_id]), 
    CONSTRAINT [FK_tblContactAddress_tblCountry] FOREIGN KEY ([tblCountry_id]) REFERENCES [tblCountry]([tblCountry_id])
)

GO

CREATE INDEX [IX_tblContactAddress_tblContact_id] ON [dbo].[tblContactAddress] ([tblContact_id])

GO

CREATE INDEX [IX_tblContactAddress_tblCountry_id] ON [dbo].[tblContactAddress] ([tblCountry_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblContactAddress] ON [dbo].[tblContactAddress] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblContactAddress'
    END