CREATE TABLE [dbo].[tblSysSetting]
(
	[tblSysSetting_id] INT NOT NULL  IDENTITY, 
    [tblSysSetting_type] INT NOT NULL, 
    [tblSysSetting_key] NVARCHAR(200) NOT NULL, 
    [tblSysSetting_value] NVARCHAR(MAX) NULL, 
    [tblSysSetting_createdUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblSysSetting_editedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    CONSTRAINT [PK_tblSysSetting] PRIMARY KEY ([tblSysSetting_id])
)

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblSysSetting] ON [dbo].[tblSysSetting] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblSysSetting'
    END