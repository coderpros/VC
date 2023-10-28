CREATE TABLE [dbo].[tblSysEmail]
(
	[tblSysEmail_id] INT NOT NULL IDENTITY,
    [tblSysEmail_type] INT NOT NULL DEFAULT 0,
    [tblSysEmail_key] NVARCHAR(50) NOT NULL, 
    [tblUser_id] INT NULL,
    [tblSysEmail_foreignRef] INT NULL, 
    [tblSysEmail_to] NVARCHAR(MAX) NULL, 
    [tblSysEmail_cc] NVARCHAR(MAX) NULL, 
    [tblSysEmail_bcc] NVARCHAR(MAX) NULL, 
    [tblSysEmail_subject] NVARCHAR(200) NULL, 
    [tblSysEmail_fromName] NVARCHAR(200) NULL, 
    [tblSysEmail_fromAddress] NVARCHAR(200) NULL, 
    [tblSysEmail_template] NVARCHAR(MAX) NULL, 
    [tblSysEmail_variables] NVARCHAR(MAX) NULL, 
    [tblSysEmail_attachments] NVARCHAR(MAX) NULL, 
    [tblSysEmail_createdUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblSysEmail_createdLocal] DATETIME NOT NULL DEFAULT GETDATE(), 
    [tblSysEmail_sentUtc] DATETIME NULL, 
    [tblSysEmail_sentLocal] DATETIME NULL, 
    [tblSysEmail_usedUtc] DATETIME NULL, 
    [tblSysEmail_usedLocal] DATETIME NULL, 
    [tblSysEmail_expiresUtc] DATETIME NULL, 
    [tblSysEmail_expiresLocal] DATETIME NULL, 
    CONSTRAINT [PK_tblSysEmail] PRIMARY KEY ([tblSysEmail_id]), 
    CONSTRAINT [FK_tblSysEmail_tblUser] FOREIGN KEY ([tblUser_id]) REFERENCES [tblUser]([tblUser_id]) 
)
GO

CREATE INDEX [IX_tblSysEmail_tblUser_id] ON [dbo].[tblSysEmail] ([tblUser_id])

GO
