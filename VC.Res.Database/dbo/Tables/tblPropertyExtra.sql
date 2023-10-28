CREATE TABLE [dbo].[tblPropertyExtra]
(
	[tblPropertyExtra_id] INT NOT NULL  IDENTITY, 
    [tblProperty_id] INT NOT NULL, 
    [tblPropertyExtra_name] NVARCHAR(100) NOT NULL, 
    [tblPropertyExtra_desc] NVARCHAR(MAX) NULL, 
    [tblPropertyExtra_priceEntryModeInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyExtra_priceEntryMode] INT NOT NULL DEFAULT 0, 
    [tblPropertyExtra_price] DECIMAL(18, 6) NOT NULL, 
    [tblPropertyExtra_commissionSubjectTo] BIT NOT NULL DEFAULT 0, 
    [tblPropertyExtra_commissionInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyExtra_commissionAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertyExtra_commissionAmount] DECIMAL(18, 6) NULL, 
    [tblPropertyExtra_commissionNote] NVARCHAR(MAX) NULL, 
    [tblPropertyExtra_taxInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyExtra_taxExempt] BIT NOT NULL DEFAULT 0, 
    [tblPropertyExtra_taxValue] DECIMAL(18, 6) NULL, 
    [tblPropertyExtra_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyExtra_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyExtra_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyExtra_editedBy] NVARCHAR(100) NULL, 
    [tblPropertyExtra_deletedUTC] DATETIME NULL, 
    [tblPropertyExtra_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyExtra] PRIMARY KEY ([tblPropertyExtra_id]), 
    CONSTRAINT [FK_tblPropertyExtra_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id])
)

GO

CREATE INDEX [IX_tblPropertyExtra_tblProperty_id] ON [dbo].[tblPropertyExtra] ([tblProperty_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyExtra] ON [dbo].[tblPropertyExtra] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyExtra'
    END
