CREATE TABLE [dbo].[tblContactTag]
(
	[tblContactTag_id] INT NOT NULL  IDENTITY, 
    [tblContact_id] INT NOT NULL, 
    [tblTag_id] INT NOT NULL, 
    [tblContactTag_order] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_tblContactTag] PRIMARY KEY ([tblContactTag_id]), 
    CONSTRAINT [FK_tblContactTag_tblContact] FOREIGN KEY ([tblContact_id]) REFERENCES [tblContact]([tblContact_id]), 
    CONSTRAINT [FK_tblContactTag_tblTag] FOREIGN KEY ([tblTag_id]) REFERENCES [tblTag]([tblTag_id]) 
)

GO

CREATE INDEX [IX_tblContactTag_tblContact_id] ON [dbo].[tblContactTag] ([tblContact_id])

GO

CREATE INDEX [IX_tblContactTag_tblTag_id] ON [dbo].[tblContactTag] ([tblTag_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblContactTag] ON [dbo].[tblContactTag] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblContactTag'
    END
