CREATE TABLE [dbo].[tblProperty]
(
	[tblProperty_id] INT NOT NULL  IDENTITY,
    [tblProperty_name] NVARCHAR(200) NOT NULL, 
    [tblProperty_displayName] NVARCHAR(200) NULL, 
    [tblProperty_websiteId] INT NULL, 
    [tblProperty_websiteURL] NVARCHAR(200) NULL, 
    [tblProperty_overview] NVARCHAR(MAX) NULL, 
    [tblProperty_otherWebsiteURLs] NVARCHAR(MAX) NULL, 
    [tblProperty_channel] INT NOT NULL DEFAULT 0, 
    [tblProperty_addressLine1] NVARCHAR(100) NOT NULL, 
    [tblProperty_addressLine2] NVARCHAR(100) NULL, 
    [tblProperty_addressLine3] NVARCHAR(100) NULL, 
    [tblProperty_addressTown] NVARCHAR(100) NOT NULL, 
    [tblProperty_addressRegion] NVARCHAR(100) NULL, 
    [tblProperty_addressPostCode] NVARCHAR(30) NULL, 
    [tblCountry_id] INT NULL,
    [tblRegion_id] INT NULL,
    [tblProperty_lat] DECIMAL(9, 6) NULL,
    [tblProperty_long] DECIMAL(9, 6) NULL,
    [tblProperty_maxGuests] INT NOT NULL DEFAULT 0,
    [tblProperty_maxGuestsAdditional] INT NOT NULL DEFAULT 0,
    [tblProperty_size] FLOAT NULL,
    [tblProperty_noBathrooms] INT NOT NULL DEFAULT 0,
    [tblPropertyGroup_id] INT NULL,
    [tblProperty_groupUseContacts] BIT NOT NULL DEFAULT 0,
    [tblProperty_licenceNo] NVARCHAR(100) NULL,
    [tblProperty_webPriceCurrencySymb] NVARCHAR(10) NULL,
    [tblProperty_webPriceCurrencyDisplay] INT NOT NULL DEFAULT 0,
    [tblProperty_webPriceMin] NVARCHAR(100) NULL,
    [tblProperty_webPriceMax] NVARCHAR(100) NULL,
    [tblProperty_webPriceType] INT NOT NULL DEFAULT 0,
    [tblProperty_saveToUmbraco] bit NOT NULL DEFAULT 0,
    [tblProperty_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblProperty_createdBy] NVARCHAR(100) NULL, 
    [tblProperty_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblProperty_editedBy] NVARCHAR(100) NULL, 
    [tblProperty_deletedUTC] DATETIME NULL, 
    [tblProperty_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblProperty] PRIMARY KEY ([tblProperty_id]), 
    CONSTRAINT [FK_tblProperty_tblCountry] FOREIGN KEY ([tblCountry_id]) REFERENCES [tblCountry]([tblCountry_id]), 
    CONSTRAINT [FK_tblProperty_tblRegion] FOREIGN KEY ([tblRegion_id]) REFERENCES [tblRegion]([tblRegion_id]), 
    CONSTRAINT [FK_tblProperty_tblPropertyGroup] FOREIGN KEY ([tblPropertyGroup_id]) REFERENCES [tblPropertyGroup]([tblPropertyGroup_id])
)

GO

CREATE INDEX [IX_tblProperty_tblCountry_id] ON [dbo].[tblProperty] ([tblCountry_id])

GO

CREATE INDEX [IX_tblProperty_tblRegion_id] ON [dbo].[tblProperty] ([tblRegion_id])

GO

CREATE INDEX [IX_tblProperty_tblPropertyGroup_id] ON [dbo].[tblProperty] ([tblPropertyGroup_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblProperty] ON [dbo].[tblProperty] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblProperty'
    END
