CREATE TABLE [dbo].[tblUserSession]
(
	[tblUserSession_id] INT NOT NULL  IDENTITY,
    [tblUserSession_type] INT NOT NULL DEFAULT 0,
    [tblUser_id] INT NOT NULL,
    [tblUserSession_key1] NVARCHAR(50) NOT NULL, 
    [tblUserSession_key2] NVARCHAR(50) NOT NULL,
    [tblUserSession_key3] NVARCHAR(50) NOT NULL,
    [tblUserSession_key4] NVARCHAR(50) NULL,
    [tblUserSession_authenticated] BIT NOT NULL DEFAULT 0,
    [tblUserSession_claimed] BIT NOT NULL DEFAULT 0,
    [tblUserSession_createdUTC] DATETIME NOT NULL DEFAULT GETDATE(), 
    [tblUserSession_createdLocal] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblUserSession_createdIP] NVARCHAR(50) NULL, 
    [tblUserSession_lastActivityUTC] DATETIME NOT NULL DEFAULT GETDATE(), 
    [tblUserSession_lastActivityLocal] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblUserSession_lastActivityIP] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_tblUserSession] PRIMARY KEY ([tblUserSession_id]), 
    CONSTRAINT [FK_tblUserSession_tblUser] FOREIGN KEY ([tblUser_id]) REFERENCES [tblUser]([tblUser_id]) 
)

GO

CREATE INDEX [IX_tblUserSession_tblUser_id] ON [dbo].[tblUserSession] ([tblUser_id])

GO
