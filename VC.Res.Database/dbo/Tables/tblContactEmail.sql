CREATE TABLE [dbo].[tblContactEmail]
(
	[tblContactEmail_id] INT NOT NULL  IDENTITY, 
    [tblContact_id] INT NOT NULL, 
    [tblContactEmail_address] NVARCHAR(200) NOT NULL, 
    [tblContactEmail_primary] BIT NOT NULL DEFAULT 0, 
    [tblContactEmail_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblContactEmail_createdBy] NVARCHAR(100) NULL,
    [tblContactEmail_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblContactEmail_editedBy] NVARCHAR(100) NULL,
    CONSTRAINT [PK_tblContactEmail] PRIMARY KEY ([tblContactEmail_id]), 
    CONSTRAINT [FK_tblContactEmail_tblContact] FOREIGN KEY ([tblContact_id]) REFERENCES [tblContact]([tblContact_id])
)

GO

CREATE INDEX [IX_tblContactEmail_tblContact_id] ON [dbo].[tblContactEmail] ([tblContact_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblContactEmail] ON [dbo].[tblContactEmail] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblContactEmail'
    END