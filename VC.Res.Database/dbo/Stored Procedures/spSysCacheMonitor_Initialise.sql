CREATE PROCEDURE [dbo].[spSysCacheMonitor_Initialise]
AS
BEGIN

	-- Get the triggers to be processed
	DECLARE @TriggersToProcess TABLE
	(
		Id      int             not null identity(1,1),
		Name    nvarchar(350)   not null
	)

	INSERT INTO @TriggersToProcess (Name)
	SELECT name FROM sysobjects WITH (NOLOCK) WHERE name like 'Trigger_CacheMonitor_%' AND type = 'TR'
	
	-- Determine first trigger to process to start loop
	DECLARE @Id int = (select min(Id) from @TriggersToProcess)

	-- Create variables for use in loop
	DECLARE @TriggerName AS nvarchar(350)
	DECLARE @TableName as nvarchar(300)

	-- Run through triggers
	WHILE (@Id is not null) 
	BEGIN      
		-- Set variables
		SET @TriggerName = (SELECT Name FROM @TriggersToProcess WHERE Id = @Id)
		SET @TableName = REPLACE(@TriggerName, 'Trigger_CacheMonitor_','')

		BEGIN TRAN
			-- Insert entry into cache monitoring table if one is not already found
			IF NOT EXISTS (SELECT tblSysCacheMonitor_table FROM dbo.tblSysCacheMonitor WITH (NOLOCK) WHERE tblSysCacheMonitor_table = @TableName) 
				 IF NOT EXISTS (SELECT tblSysCacheMonitor_table FROM dbo.tblSysCacheMonitor WITH (TABLOCKX) WHERE tblSysCacheMonitor_table = @TableName) 
					 INSERT INTO dbo.tblSysCacheMonitor (tblSysCacheMonitor_table) VALUES (@TableName)
		COMMIT TRAN		  
		           
		-- Select next record, this will break the loop if next record is not found.
		SET @Id = (SELECT Id FROM @TriggersToProcess WHERE Id = @Id + 1)
	END

END