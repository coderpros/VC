CREATE TABLE [dbo].[tblSysAudit]
(
	[tblSysAudit_id] INT NOT NULL  IDENTITY, 
    [tblSysAudit_type] NVARCHAR(200) NOT NULL, 
    [tblSysAudit_foreignId] INT NOT NULL, 
    [tblSysAudit_action] INT NOT NULL, 
    [tblSysAudit_newData] NVARCHAR(MAX) NULL, 
    [tblSysAudit_oldData] NVARCHAR(MAX) NULL, 
    [tblSysAudit_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblSysAudit_createdLocal] DATETIME NOT NULL DEFAULT GETDATE(), 
    [tblSysAudit_createdBy] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_tblSysAudit] PRIMARY KEY ([tblSysAudit_id])
)