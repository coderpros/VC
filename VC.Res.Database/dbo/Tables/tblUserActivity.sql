CREATE TABLE [dbo].[tblUserActivity]
(
	[tblUserActivity_id] INT NOT NULL  IDENTITY, 
    [tblUser_id] INT NULL, 
    [tblUserActivity_email] NVARCHAR(200) NULL,
    [tblUserActivity_actionGroup] INT NOT NULL,
    [tblUserActivity_actionType] INT NOT NULL,
    [tblUserActivity_actionText] NVARCHAR(200) NULL,
    [tblUserActivity_refItemId1] INT NULL,
    [tblUserActivity_refItemId2] INT NULL,
    [tblUserActivity_refItemId3] INT NULL,
    [tblUserActivity_refItemId4] INT NULL,
    [tblUserActivity_refItemId5] INT NULL,
    [tblSysAudit_id] INT NULL,
    [tblUserActivity_success] BIT NOT NULL DEFAULT (0),
    [tblUserActivity_note] NVARCHAR(MAX) NULL, 
    [tblUserActivity_ipAddress] NVARCHAR(50) NULL, 
    [tblUserActivity_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblUserActivity_createdLocal] DATETIME NOT NULL DEFAULT GETDATE(), 
    CONSTRAINT [FK_tblUserActivity_tblUser] FOREIGN KEY ([tblUser_id]) REFERENCES [tblUser]([tblUser_id]), 
    CONSTRAINT [PK_tblUserActivity] PRIMARY KEY ([tblUserActivity_id]), 
    CONSTRAINT [FK_tblUserActivity_tblSysAudit] FOREIGN KEY ([tblSysAudit_id]) REFERENCES [tblSysAudit]([tblSysAudit_id])
)

GO

CREATE INDEX [IX_tblUserActivity_tblUser_id] ON [dbo].[tblUserActivity] ([tblUser_id])

GO

CREATE INDEX [IX_tblUserActivity_tblSysAudit_id] ON [dbo].[tblUserActivity] ([tblSysAudit_id])
