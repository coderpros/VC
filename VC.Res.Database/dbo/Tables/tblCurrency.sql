CREATE TABLE [dbo].[tblCurrency]
(
	[tblCurrency_id] INT NOT NULL  IDENTITY, 
    [tblCurrency_name] NVARCHAR(100) NOT NULL, 
    [tblCurrency_code] NVARCHAR(5) NOT NULL, 
    [tblCurrency_symbol] NVARCHAR(10) NOT NULL, 
    [tblCurrency_symbolAfter] BIT NOT NULL DEFAULT 0, 
    [tblCurrency_default] BIT NOT NULL DEFAULT 0, 
	[tblCurrency_order] INT NOT NULL DEFAULT 0, 
    [tblCurrency_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblCurrency_createdBy] NVARCHAR(100) NULL, 
    [tblCurrency_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblCurrency_editedBy] NVARCHAR(100) NULL, 
    [tblCurrency_deletedUTC] DATETIME NULL, 
    [tblCurrency_deletedBy] NVARCHAR(100) NULL,
    CONSTRAINT [PK_tblCurrency] PRIMARY KEY ([tblCurrency_id])
)

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblCurrency] ON [dbo].[tblCurrency] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblCurrency'
    END
