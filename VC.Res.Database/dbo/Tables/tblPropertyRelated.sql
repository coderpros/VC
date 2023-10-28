CREATE TABLE [dbo].[tblPropertyRelated]
(
	[tblPropertyRelated_id] INT NOT NULL  IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblPropertyRelated_type] INT NOT NULL DEFAULT 0, 
    [tblProperty_relatedId] INT NOT NULL, 
    [tblPropertyRelated_order] INT NOT NULL DEFAULT 0, 
    [tblPropertyRelated_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyRelated_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyRelated_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyRelated_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyRelated] PRIMARY KEY ([tblPropertyRelated_id]), 
    CONSTRAINT [FK_tblPropertyRelated_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id]), 
    CONSTRAINT [FK_tblPropertyRelated_tblProperty_related] FOREIGN KEY ([tblProperty_relatedId]) REFERENCES [tblProperty]([tblProperty_id])
)

GO

CREATE INDEX [IX_tblPropertyRelated_tblProperty_id] ON [dbo].[tblPropertyRelated] ([tblProperty_id])

GO

CREATE INDEX [IX_tblPropertyRelated_tblProperty_relatedId] ON [dbo].[tblPropertyRelated] ([tblProperty_relatedId])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyRelated] ON [dbo].[tblPropertyRelated] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyRelated'
    END
