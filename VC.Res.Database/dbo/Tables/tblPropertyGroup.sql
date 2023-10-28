CREATE TABLE [dbo].[tblPropertyGroup]
(
	[tblPropertyGroup_id] INT NOT NULL  IDENTITY, 
    [tblPropertyGroup_name] NVARCHAR(100) NOT NULL, 
    [tblPropertyGroup_desc] NVARCHAR(MAX) NULL,
    [tblPropertyGroup_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyGroup_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyGroup_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyGroup_editedBy] NVARCHAR(100) NULL, 
    [tblPropertyGroup_deletedUTC] DATETIME NULL, 
    [tblPropertyGroup_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyGroup] PRIMARY KEY ([tblPropertyGroup_id]), 
)

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyGroup] ON [dbo].[tblPropertyGroup] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyGroup'
    END
