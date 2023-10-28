CREATE TABLE [dbo].[tblSysChangeLog]
(
    [tblSysChangeLog_id] INT NOT NULL  IDENTITY, 
    [tblSysChangeLog_version] FLOAT NOT NULL , 
    [tblSysChangeLog_date] DATETIME NOT NULL, 
    [tblSysChangeLog_applied] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblSysChangeLog_additions] NVARCHAR(MAX) NULL, 
    [tblSysChangeLog_fixes] NVARCHAR(MAX) NULL, 
    [tblSysChangeLog_note] NVARCHAR(MAX) NULL, 
    [tblSysChangeLog_patched] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_tblSysChangeLog] PRIMARY KEY ([tblSysChangeLog_id])
)