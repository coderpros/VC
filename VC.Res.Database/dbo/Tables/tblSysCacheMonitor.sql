CREATE TABLE [dbo].[tblSysCacheMonitor]
(
	[tblSysCacheMonitor_table] NVARCHAR(300) NOT NULL, 
    [tblSysCacheMonitor_created] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblSysCacheMonitor_changeId] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_tblSysCacheMonitor] PRIMARY KEY CLUSTERED ([tblSysCacheMonitor_table] ASC) 
)
