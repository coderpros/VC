CREATE TABLE [dbo].[tblPropertyContact]
(
	[tblPropertyContact_id] INT NOT NULL IDENTITY , 
    [tblProperty_id] INT NULL, 
    [tblPropertyGroup_id] INT NULL, 
    [tblContact_id] INT NOT NULL,
    [tblPropertyContact_configPrimary] BIT NOT NULL DEFAULT 0,
    [tblPropertyContact_categories] NVARCHAR(MAX) NULL,
    [tblPropertyContact_papmInfo] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_papmRates] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_papmAvailability] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_papmBookings] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_papmBookingConfirmation] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_papmRemitSlip] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_notifiInfo] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_notifiRates] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_notifiAvailability] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_notifiBookings] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_notifiBookingConfirmation] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_notifiRemitSlip] BIT NOT NULL DEFAULT 0, 
    [tblPropertyContact_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyContact_createdBy] NVARCHAR(100) NULL, 
    [tblPropertyContact_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertyContact_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertyContact] PRIMARY KEY ([tblPropertyContact_id]), 
    CONSTRAINT [FK_tblPropertyContact_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id]), 
    CONSTRAINT [FK_tblPropertyContact_tblPropertyGroup] FOREIGN KEY ([tblPropertyGroup_id]) REFERENCES [tblPropertyGroup]([tblPropertyGroup_id]), 
    CONSTRAINT [FK_tblPropertyContact_tblContact] FOREIGN KEY ([tblContact_id]) REFERENCES [tblContact]([tblContact_id])
)

GO

CREATE INDEX [IX_tblPropertyContact_tblProperty_id] ON [dbo].[tblPropertyContact] ([tblProperty_id])

GO

CREATE INDEX [IX_tblPropertyContact_tblPropertyGroup_id] ON [dbo].[tblPropertyContact] ([tblPropertyGroup_id])

GO

CREATE INDEX [IX_tblPropertyContact_tblContact_id] ON [dbo].[tblPropertyContact] ([tblContact_id])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblPropertyContact] ON [dbo].[tblPropertyContact] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblPropertyContact'
    END