use [b-vc-2517-local]
CREATE TABLE [dbo].[tblUser]
(
	[tblUser_id] INT NOT NULL  IDENTITY,
    [tblUser_email] NVARCHAR(100) NOT NULL,
    [tblUser_firstName] NVARCHAR(100) NULL, 
    [tblUser_lastName] NVARCHAR(100) NULL, 
    [tblUser_pwdSalt] NVARCHAR(100) NULL, 
    [tblUser_pwd] NVARCHAR(100) NULL, 
    [tblUser_pwdLastChangedUTC] DATETIME NULL, 
    [tblUser_pwdLastChangedLocal] DATETIME NULL, 
    [tblUser_twoFAEnabled] BIT NOT NULL DEFAULT (0),
    [tblUser_twoFAMethod] INT NOT NULL DEFAULT 0,
    [tblUser_telMobile] NVARCHAR(100) NULL, 
    [tblUser_telMobileVerified] BIT NOT NULL DEFAULT (0),
    [tblUser_accessSysAdmin] BIT NOT NULL DEFAULT (0),
    [tblUser_enabled] BIT NOT NULL DEFAULT (0),  
    [tblUser_lastLoginUTC] DATETIME NULL, 
    [tblUser_lastLoginLocal] DATETIME NULL, 
    [tblUser_lastLoginIP] NVARCHAR(100) NULL, 
    [tblUser_failedLoginTotal] INT NOT NULL DEFAULT 0, 
    [tblUser_failedLoginLastUTC] DATETIME NULL, 
    [tblUser_failedLoginLastLocal] DATETIME NULL, 
    [tblUser_failedLoginLockUTC] DATETIME NULL, 
    [tblUser_failedLoginLockLocal] DATETIME NULL, 
    [tblUser_createdUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblUser_createdLocal] DATETIME NOT NULL DEFAULT GETDATE(), 
    [tblUser_createdBy] NVARCHAR(200) NULL, 
    [tblUser_editedUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblUser_editedLocal] DATETIME NOT NULL DEFAULT GETDATE(), 
    [tblUser_editedBy] NVARCHAR(200) NULL, 
    [tblUser_deletedUTC] DATETIME NULL, 
    [tblUser_deletedLocal] DATETIME NULL, 
    [tblUser_deletedBy] NVARCHAR(200) NULL, 
    CONSTRAINT [PK_tblUser] PRIMARY KEY ([tblUser_id])
)
GO

CREATE INDEX [IX_tblUser_tblUser_email] ON [dbo].[tblUser] ([tblUser_email])

GO

CREATE TRIGGER [dbo].[Trigger_CacheMonitor_tblUser] ON [dbo].[tblUser] FOR INSERT, UPDATE, DELETE 
	AS 
	BEGIN
		SET NOCOUNT ON
		EXEC [dbo].[spSysCacheMonitor_Update] N'tblUser'
    END
