CREATE TABLE [dbo].[tblTag]
(
	[tblTag_id] INT NOT NULL  IDENTITY, 
    [tblTag_type] INT NOT NULL DEFAULT 0, 
    [tblTag_name] NVARCHAR(200) NOT NULL, 
    [tblTag_desc] NVARCHAR(MAX) NULL,
    [tblTag_icon] INT NOT NULL DEFAULT 0,
    [tblTag_propertyCategories] NVARCHAR(MAX) NULL,
    [tblTag_order] INT NOT NULL DEFAULT 0,
    [tblTag_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblTag_createdBy] NVARCHAR(200) NULL, 
    [tblTag_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblTag_editedBy] NVARCHAR(200) NULL,
    [tblTag_deletedUTC] DATETIME NULL, 
    [tblTag_deletedBy] NVARCHAR(100) NULL,
    CONSTRAINT [PK_tblTag] PRIMARY KEY ([tblTag_id])
)

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblTag] ON [dbo].[tblTag] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblTag'
    END
