CREATE TABLE [dbo].[tblPropertySeasonDate]
(
	[tblPropertySeasonDate_id] INT NOT NULL  IDENTITY, 
    [tblPropertySeason_id] INT NOT NULL,
    [tblPropertySeasonDate_from] DATETIME NOT NULL, 
    [tblPropertySeasonDate_to] DATETIME NOT NULL, 
    [tblPropertySeasonDate_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertySeasonDate_createdBy] NVARCHAR(100) NULL, 
    [tblPropertySeasonDate_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblPropertySeasonDate_editedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblPropertySeasonDate] PRIMARY KEY ([tblPropertySeasonDate_id]), 
    CONSTRAINT [FK_tblPropertySeasonDate_tblPropertySeason] FOREIGN KEY ([tblPropertySeason_id]) REFERENCES [tblPropertySeason]([tblPropertySeason_id])
)

GO

CREATE INDEX [IX_tblPropertySeasonDate_tblPropertySeason_id] ON [dbo].[tblPropertySeasonDate] ([tblPropertySeason_id])
