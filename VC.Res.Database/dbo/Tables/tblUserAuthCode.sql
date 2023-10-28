CREATE TABLE [dbo].[tblUserAuthCode]
(
	[tblUserAuthCode_id] INT NOT NULL  IDENTITY,
    [tblUser_id] INT NOT NULL,
    [tblUserAuthCode_code] NVARCHAR(6) NOT NULL, 
    [tblUserAuthCode_expiresUTC] DATETIME NOT NULL, 
    [tblUserAuthCode_expiresLocal] DATETIME NOT NULL, 
    CONSTRAINT [FK_tblUserAuthCode_tblUser] FOREIGN KEY ([tblUser_id]) REFERENCES [tblUser]([tblUser_id]), 
    CONSTRAINT [PK_tblUserAuthCode] PRIMARY KEY ([tblUserAuthCode_id]), 
)

GO

CREATE INDEX [IX_tblUserAuthCode_tblUser_id] ON [dbo].[tblUserAuthCode] ([tblUser_id])
