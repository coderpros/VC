CREATE TABLE [dbo].[tblContact]
(
	[tblContact_id] INT NOT NULL  IDENTITY,
    [tblContact_zohoId] NVARCHAR(200) NULL,
    [tblContact_companyName] NVARCHAR(100) NULL, 
    [tblContact_title] NVARCHAR(20) NULL, 
    [tblContact_firstName] NVARCHAR(50) NULL, 
    [tblContact_middleName] NVARCHAR(50) NULL, 
    [tblContact_lastName] NVARCHAR(50) NULL,
    [tblContact_websiteURL] NVARCHAR(MAX) NULL,
    [tblContact_prefContactMethod] INT NOT NULL DEFAULT 0,
    [tblContact_categories] NVARCHAR(MAX) NULL,
    [tblContact_note] NVARCHAR(MAX) NULL,
    [tblContact_agentAmountType] INT NOT NULL DEFAULT 0,
    [tblContact_agentAmount] DECIMAL(18,6) NULL,
    [tblContact_agentPaymentPoint] INT NOT NULL DEFAULT 0,
    [tblContact_agentPaymentDeposit] DECIMAL(18,6) NULL,
    [tblContact_agentPaymentInterim] DECIMAL(18,6) NULL,
    [tblContact_agentPaymentBalance] DECIMAL(18,6) NULL,
    [tblContact_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblContact_createdBy] NVARCHAR(100) NULL, 
    [tblContact_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblContact_editedBy] NVARCHAR(100) NULL, 
    [tblContact_deletedUTC] DATETIME NULL, 
    [tblContact_deletedBy] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_tblContact] PRIMARY KEY ([tblContact_id])
)

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblContact] ON [dbo].[tblContact] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblContact'
    END