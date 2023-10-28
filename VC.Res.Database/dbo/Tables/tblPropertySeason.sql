CREATE TABLE [dbo].[tblPropertySeason]
(
	[tblPropertySeason_id] INT NOT NULL IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblPropertySeason_name] NVARCHAR(100) NULL, 
    [tblPropertySeason_noteInt] NVARCHAR(MAX) NULL, 
    [tblPropertySeason_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertySeason_createdBy] NVARCHAR(100) NULL, 
    [tblPropertySeason_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertySeason_editedBy] NVARCHAR(100) NULL, 
    [tblPropertySeason_deletedUTC] DATETIME NULL, 
    [tblPropertySeason_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertySeason] PRIMARY KEY ([tblPropertySeason_id]), 
    CONSTRAINT [FK_tblPropertySeason_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id]) 
)

GO

CREATE INDEX [IX_tblPropertySeason_tblProperty_id] ON [dbo].[tblPropertySeason] ([tblProperty_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertySeason] ON [dbo].[tblPropertySeason] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertySeason'
    END

