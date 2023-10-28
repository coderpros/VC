CREATE TABLE [dbo].[tblPropertyAvailability]
(
	[tblPropertyAvailability_id] INT NOT NULL  IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblPropertyAvailability_night] DATETIME NOT NULL, 
    [tblPropertyAvailability_state] INT NOT NULL, 
    [tblPropertyAvailability_note] NVARCHAR(MAX) NULL, 
    [tblPropertyAvailability_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyAvailability_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyAvailability_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyAvailability_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyAvailability] PRIMARY KEY ([tblPropertyAvailability_id]), 
    CONSTRAINT [FK_tblPropertyAvailability_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id])
)

GO

CREATE INDEX [IX_tblPropertyAvailability_tblProperty_id] ON [dbo].[tblPropertyAvailability] ([tblProperty_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyAvailability] ON [dbo].[tblPropertyAvailability] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyAvailability'
    END
