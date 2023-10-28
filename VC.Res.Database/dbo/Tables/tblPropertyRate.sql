CREATE TABLE [dbo].[tblPropertyRate]
(
	[tblPropertyRate_id] INT NOT NULL  IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblPropertySeason_id] INT NULL, 
    [tblPropertyRate_parentId] INT NULL, 
    [tblPropertyRate_provisional] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRate_reqReview] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRate_dateArrive] DATETIME NOT NULL, 
    [tblPropertyRate_dateDepart] DATETIME NOT NULL, 
    [tblPropertyRate_noNights] INT NOT NULL,
    [tblPropertyRate_minPartySize] INT NOT NULL DEFAULT 0,
    [tblPropertyRate_available] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRate_pricePOA] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRate_priceEntryMode] INT NOT NULL DEFAULT 0, 
    [tblPropertyRate_price] DECIMAL(18, 6) NOT NULL, 
    [tblPropertyRate_commissionAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertyRate_commissionAmount] DECIMAL(18, 6) NOT NULL, 
    [tblPropertyRate_commissionNote] NVARCHAR(MAX) NULL, 
    [tblPropertyRate_taxExempt] BIT NOT NULL DEFAULT 0, 
    [tblPropertyRate_taxValue] DECIMAL(18, 6) NOT NULL,
    [tblPropertyRate_discount] BIT NOT NULL DEFAULT 0,
    [tblPropertyRate_discountNights] INT NOT NULL DEFAULT 0, 
    [tblPropertyRate_discountEntryMode] INT NOT NULL DEFAULT 0, 
    [tblPropertyRate_discountAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertyRate_discountAmount] DECIMAL(18, 6) NOT NULL, 
    [tblPropertyRate_discountNote] NVARCHAR(MAX) NULL, 
    [tblPropertyRate_extraExcludes] NVARCHAR(MAX) NULL, 
    [tblPropertyRate_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyRate_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyRate_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyRate_editedBy] NVARCHAR(100) NULL, 
    [tblPropertyRate_deletedUTC] DATETIME NULL, 
    [tblPropertyRate_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyRate] PRIMARY KEY ([tblPropertyRate_id]), 
    CONSTRAINT [FK_tblPropertyRate_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id]), 
    CONSTRAINT [FK_tblPropertyRate_tblPropertySeason] FOREIGN KEY ([tblPropertySeason_id]) REFERENCES [tblPropertySeason]([tblPropertySeason_id]), 
    CONSTRAINT [FK_tblPropertyRate_tblPropertyRate] FOREIGN KEY ([tblPropertyRate_parentId]) REFERENCES [tblPropertyRate]([tblPropertyRate_id])
)

GO

CREATE INDEX [IX_tblPropertyRate_tblProperty_id] ON [dbo].[tblPropertyRate] ([tblProperty_id])

GO

CREATE INDEX [IX_tblPropertyRate_tblPropertySeason_id] ON [dbo].[tblPropertyRate] ([tblPropertySeason_id])

GO

CREATE INDEX [IX_tblPropertyRate_tblPropertyRate_parentId] ON [dbo].[tblPropertyRate] ([tblPropertyRate_parentId])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyRate] ON [dbo].[tblPropertyRate] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyRate'
    END