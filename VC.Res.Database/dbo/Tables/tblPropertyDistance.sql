CREATE TABLE [dbo].[tblPropertyDistance]
(
	[tblPropertyDistance_id] INT NOT NULL  IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblPropertyDistance_type] INT NOT NULL DEFAULT 0, 
    [tblPropertyDistance_name] NVARCHAR(100) NULL, 
    [tblPropertyDistance_desc] NVARCHAR(MAX) NULL, 
    [tblPropertyDistance_distanceKM] FLOAT NOT NULL DEFAULT 0, 
    [tblPropertyDistance_lat] DECIMAL(9, 6) NULL,
    [tblPropertyDistance_long] DECIMAL(9, 6) NULL,
    [tblPropertyDistance_minByWalk] INT NULL , 
    [tblPropertyDistance_minByDrive] INT NULL, 
    [tblPropertyDistance_minByBoat] INT NULL, 
    [tblPropertyDistance_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyDistance_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyDistance_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyDistance_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyDistance] PRIMARY KEY ([tblPropertyDistance_id]), 
    CONSTRAINT [FK_tblPropertyDistance_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id])
)

GO

CREATE INDEX [IX_tblPropertyDistance_tblProperty_id] ON [dbo].[tblPropertyDistance] ([tblProperty_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyDistance] ON [dbo].[tblPropertyDistance] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyDistance'
    END
