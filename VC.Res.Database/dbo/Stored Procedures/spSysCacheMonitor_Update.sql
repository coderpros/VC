CREATE PROCEDURE [dbo].[spSysCacheMonitor_Update]
	@tableName NVARCHAR(300) 
AS

BEGIN 
    UPDATE dbo.tblSysCacheMonitor WITH (ROWLOCK) SET tblSysCacheMonitor_changeId = tblSysCacheMonitor_changeId + 1 
    WHERE tblSysCacheMonitor_table = @tableName
END