CREATE TABLE [dbo].[tblUserIP]
(
	[tblUserIP_id] INT NOT NULL  IDENTITY, 
    [tblUser_id] INT NOT NULL, 
    [tblUserIP_ipAddress] NVARCHAR(100) NULL, 
    [tblUserIP_authorised] BIT NOT NULL DEFAULT (0), 
    [tblUserIP_lastLoginUTC] DATETIME NOT NULL, 
    [tblUserIP_lastLoginLocal] DATETIME NOT NULL, 
    [tblUserIP_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblUserIP_createdLocal] DATETIME NOT NULL DEFAULT GETDATE(), 
    CONSTRAINT [FK_tblUserIP_tblUser] FOREIGN KEY ([tblUser_id]) REFERENCES [tblUser]([tblUser_id]), 
    CONSTRAINT [PK_tblUserIP] PRIMARY KEY ([tblUserIP_id])
)

GO

CREATE INDEX [IX_tblUserIP_tblUser_id] ON [dbo].[tblUserIP] ([tblUser_id])
