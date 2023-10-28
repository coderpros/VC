CREATE TABLE [dbo].[tblRegion]
(
	[tblRegion_id] INT NOT NULL  IDENTITY, 
    [tblCountry_id] INT NOT NULL, 
    [tblRegion_websiteId] INT NULL,
    [tblRegion_name] NVARCHAR(100) NOT NULL, 
    [tblRegion_desc] NVARCHAR(200) NULL,
    [tblRegion_createdUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblRegion_createdBy] NVARCHAR(100) NULL, 
    [tblRegion_editedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblRegion_editedBy] NVARCHAR(100) NULL, 
    [tblRegion_deletedUtc] DATETIME NULL, 
    [tblRegion_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblRegion] PRIMARY KEY ([tblRegion_id]), 
    CONSTRAINT [FK_tblRegion_tblCountry] FOREIGN KEY ([tblCountry_id]) REFERENCES [tblCountry]([tblCountry_id])
)

GO

CREATE INDEX [IX_tblRegion_tblCountry_id] ON [dbo].[tblRegion] ([tblCountry_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblRegion] ON [dbo].[tblRegion] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblRegion'
    END
