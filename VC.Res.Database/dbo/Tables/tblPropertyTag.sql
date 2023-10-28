CREATE TABLE [dbo].[tblPropertyTag]
(
	[tblPropertyTag_id] INT NOT NULL  IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblTag_id] INT NOT NULL, 
    [tblPropertyTag_category] INT NOT NULL DEFAULT 0, 
    [tblPropertyTag_desc] NVARCHAR(MAX) NULL, 
    [tblPropertyTag_order] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_tblPropertyTag] PRIMARY KEY ([tblPropertyTag_id]), 
    CONSTRAINT [FK_tblPropertyTag_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id]), 
    CONSTRAINT [FK_tblPropertyTag_tblTag] FOREIGN KEY ([tblTag_id]) REFERENCES [tblTag]([tblTag_id]) 
)

GO

CREATE INDEX [IX_tblPropertyTag_tblProperty_id] ON [dbo].[tblPropertyTag] ([tblProperty_id])

GO

CREATE INDEX [IX_tblPropertyTag_tblTag_id] ON [dbo].[tblPropertyTag] ([tblTag_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyTag] ON [dbo].[tblPropertyTag] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyTag'
    END
