CREATE TABLE [dbo].[tblContactTel]
(
	[tblContactTel_id] INT NOT NULL  IDENTITY, 
    [tblContact_id] INT NOT NULL, 
	[tblContactTel_countryCode] NVARCHAR(10) NOT NULL, 
    [tblContactTel_no] NVARCHAR(50) NOT NULL, 
    [tblContactTel_primary] BIT NOT NULL DEFAULT 0, 
    [tblContactTel_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblContactTel_createdBy] NVARCHAR(100) NULL, 
    [tblContactTel_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblContactTel_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblContactTel] PRIMARY KEY ([tblContactTel_id]), 
    CONSTRAINT [FK_tblContactTel_tblContact] FOREIGN KEY ([tblContact_id]) REFERENCES [tblContact]([tblContact_id])
)

GO

CREATE INDEX [IX_tblContactTel_tblContact_id] ON [dbo].[tblContactTel] ([tblContact_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblContactTel] ON [dbo].[tblContactTel] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblContactTel'
    END