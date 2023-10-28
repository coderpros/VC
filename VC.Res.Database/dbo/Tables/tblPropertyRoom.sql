CREATE TABLE [dbo].[tblPropertyRoom]
(
	[tblPropertyRoom_id] INT NOT NULL  IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblPropertyRoom_type] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_name] NVARCHAR(100) NULL, 
    [tblPropertyRoom_desc] NVARCHAR(200) NULL, 
    [tblPropertyRoom_bedsDouble] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_bedsTwinDouble] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_bedsTwin] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_bedsSingle] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_bedsBunk] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_bedsSofa] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_bedsChild] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_ensuite] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_accessInside] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_accessOutside] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_note] NVARCHAR(MAX) NULL,
    [tblPropertyRoom_order] INT NOT NULL DEFAULT 0, 
    [tblPropertyRoom_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyRoom_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyRoom_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyRoom_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyRoom] PRIMARY KEY ([tblPropertyRoom_id]), 
    CONSTRAINT [FK_tblPropertyRoom_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id])
)

GO

CREATE INDEX [IX_tblPropertyRoom_tblProperty_id] ON [dbo].[tblPropertyRoom] ([tblProperty_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyRoom] ON [dbo].[tblPropertyRoom] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyRoom'
    END
