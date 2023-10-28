CREATE TABLE [dbo].[tblPropertyConfig]
(
	[tblPropertyConfig_id] INT NOT NULL  IDENTITY, 
    [tblPropertyConfig_parentId] INT NULL, 
    [tblContact_id] INT NULL, 
    [tblProperty_id] INT NULL, 
    [tblPropertySeason_id] INT NULL, 
    [tblPropertyConfig_descForQuoteInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_descForQuote] NVARCHAR(MAX) NULL, 
    [tblPropertyConfig_houseRulesInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_houseRules] NVARCHAR(MAX) NULL, 
    [tblPropertyConfig_inclusionsInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_inclusions] NVARCHAR(MAX) NULL, 
    [tblPropertyConfig_defAvailStateInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_defAvailState] INT NOT NULL, 
    [tblPropertyConfig_reqBookingApvlInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_reqBookingApvl] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_priceEntryModeInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_priceEntryMode] INT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_currencyInh] BIT NOT NULL DEFAULT 0, 
    [tblCurrency_id] INT NULL, 
    [tblPropertyConfig_taxInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_taxExempt] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_taxValue] DECIMAL(18, 6) NULL,
    [tblPropertyConfig_taxNo] NVARCHAR(200) NULL,
    [tblPropertyConfig_bankInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_bankAccName] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAccNo] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAccSort] NVARCHAR(50) NULL, 
    [tblPropertyConfig_bankAccIBAN] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAccBIC] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAddress1] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAddress2] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAddress3] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAddressTown] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAddressCounty] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAddressPost] NVARCHAR(100) NULL, 
    [tblPropertyConfig_bankAddressCountry] NVARCHAR(100) NULL, 
    [tblPropertyConfig_commissionInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_commissionAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_commissionAmount] DECIMAL(18, 6) NULL, 
    [tblPropertyConfig_commissionNote] NVARCHAR(MAX) NULL, 
    [tblPropertyConfig_checkinInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_checkin] TIME NULL, 
    [tblPropertyConfig_checkoutInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_checkout] TIME NULL, 
    [tblPropertyConfig_changeOverDayInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_changeOverDay] INT NULL, 
    [tblPropertyConfig_minRentalInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_minRentalDays] INT NULL, 
    [tblPropertyConfig_minRentalNote] NVARCHAR(MAX) NULL, 
    [tblPropertyConfig_nightlyPriceInh] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_nightlyPrice] DECIMAL(18, 6) NULL, 
    [tblPropertyConfig_paySchInh] BIT NOT NULL, 
    [tblPropertyConfig_paySchDeposReq] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_paySchDeposAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_paySchDeposAmount] DECIMAL(18, 6) NULL, 
    [tblPropertyConfig_paySchInterReq] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_paySchInterAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_paySchInterAmount] DECIMAL(18, 6) NULL, 
    [tblPropertyConfig_paySchInterDays] INT NULL, 
    [tblPropertyConfig_paySchBalDays] INT NULL, 
    [tblPropertyConfig_paySecDeposInh] BIT NOT NULL, 
    [tblPropertyConfig_paySecDeposReq] BIT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_paySecDeposAmountType] INT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_paySecDeposAmount] DECIMAL(18, 6) NULL, 
    [tblPropertyConfig_paySecDeposCalcFrom] INT NOT NULL DEFAULT 0, 
    [tblPropertyConfig_paySecDeposDaysBefore] INT NULL, 
    [tblPropertyConfig_paySecDeposDaysAfter] INT NULL, 
    [tblPropertyConfig_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyConfig_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyConfig_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyConfig_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyConfig] PRIMARY KEY ([tblPropertyConfig_id]), 
    CONSTRAINT [FK_tblPropertyConfig_tblContact] FOREIGN KEY ([tblContact_id]) REFERENCES [tblContact]([tblContact_id]), 
    CONSTRAINT [FK_tblPropertyConfig_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id]), 
    CONSTRAINT [FK_tblPropertyConfig_tblCurrency] FOREIGN KEY ([tblCurrency_id]) REFERENCES [tblCurrency]([tblCurrency_id]), 
    CONSTRAINT [FK_tblPropertyConfig_tblPropertyConfig] FOREIGN KEY ([tblPropertyConfig_parentId]) REFERENCES [tblPropertyConfig]([tblPropertyConfig_id]), 
    CONSTRAINT [FK_tblPropertyConfig_tblPropertySeason] FOREIGN KEY ([tblPropertySeason_id]) REFERENCES [tblPropertySeason]([tblPropertySeason_id])
)

GO

CREATE INDEX [IX_tblPropertyConfig_tblContact_id] ON [dbo].[tblPropertyConfig] ([tblContact_id])

GO

CREATE INDEX [IX_tblPropertyConfig_tblProperty_id] ON [dbo].[tblPropertyConfig] ([tblProperty_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyConfig] ON [dbo].[tblPropertyConfig] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyConfig'
    END


GO

CREATE INDEX [IX_tblPropertyConfig_tblCurrency_id] ON [dbo].[tblPropertyConfig] ([tblCurrency_id])

GO

CREATE INDEX [IX_tblPropertyConfig_tblPropertyConfig_parentId] ON [dbo].[tblPropertyConfig] ([tblPropertyConfig_parentId])

GO

CREATE INDEX [IX_tblPropertyConfig_tblPropertySeason_id] ON [dbo].[tblPropertyConfig] ([tblPropertySeason_id])
