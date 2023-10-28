CREATE TABLE [dbo].[tblPropertySeasonExtra]
(
	[tblPropertySeasonExtra_id] INT NOT NULL  IDENTITY, 
    [tblPropertySeason_id] INT NOT NULL, 
    [tblPropertyExtra_id] INT NULL,
    [tblPropertySeasonExtra_name] NVARCHAR(100) NOT NULL, 
    [tblPropertySeasonExtra_desc] NVARCHAR(MAX) NULL, 
    [tblPropertySeasonExtra_priceEntryMode] INT NOT NULL DEFAULT 0, 
    [tblPropertySeasonExtra_price] DECIMAL(18, 6) NOT NULL, 
    [tblPropertySeasonExtra_commissionSubjectTo] BIT NOT NULL DEFAULT 0, 
    [tblPropertySeasonExtra_commissionAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertySeasonExtra_commissionAmount] DECIMAL(18, 6) NOT NULL, 
    [tblPropertySeasonExtra_commissionNote] NVARCHAR(MAX) NULL, 
    [tblPropertySeasonExtra_taxExempt] BIT NOT NULL DEFAULT 0, 
    [tblPropertySeasonExtra_taxValue] DECIMAL(18, 6) NOT NULL , 
    [tblPropertySeasonExtra_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertySeasonExtra_createdBy] NVARCHAR(100) NULL, 
    [tblPropertySeasonExtra_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertySeasonExtra_editedBy] NVARCHAR(100) NULL, 
    [tblPropertySeasonExtra_deletedUTC] DATETIME NULL, 
    [tblPropertySeasonExtra_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertySeasonExtra] PRIMARY KEY ([tblPropertySeasonExtra_id]), 
    CONSTRAINT [FK_tblPropertySeasonExtra_tblPropertySeason] FOREIGN KEY ([tblPropertySeason_id]) REFERENCES [tblPropertySeason]([tblPropertySeason_id]), 
    CONSTRAINT [FK_tblPropertySeasonExtra_tblPropertyExtra] FOREIGN KEY ([tblPropertyExtra_id]) REFERENCES [tblPropertyExtra]([tblPropertyExtra_id])
)

GO

CREATE INDEX [IX_tblPropertySeasonExtra_tblPropertySeason_id] ON [dbo].[tblPropertySeasonExtra] ([tblPropertySeason_id])

GO

CREATE INDEX [IX_tblPropertySeasonExtra_tblPropertyExtra_id] ON [dbo].[tblPropertySeasonExtra] ([tblPropertyExtra_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertySeasonExtra] ON [dbo].[tblPropertySeasonExtra] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertySeasonExtra'
    END
