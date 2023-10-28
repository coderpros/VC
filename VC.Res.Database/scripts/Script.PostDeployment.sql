--/* 0.001 Setup */
--/*-----------------------------------------------------------------------*/
--IF NOT EXISTS (SELECT 1 FROM tblSysChangeLog WHERE tblSysChangeLog_version = 0.001)
--    BEGIN

--	   SET XACT_ABORT ON

--	   BEGIN TRANSACTION

--    	/* Add version history */
--	   /*-----------------------------------------------------------------------*/
--	   INSERT INTO [tblSysChangeLog] ([tblSysChangeLog_version],[tblSysChangeLog_date],[tblSysChangeLog_additions],[tblSysChangeLog_fixes],[tblSysChangeLog_note]) VALUES (0.001,CAST('20220201' AS DATETIME),'','','Initial build');
--	   /*-----------------------------------------------------------------------*/

--	   COMMIT TRANSACTION

--	   SET XACT_ABORT OFF
--    END
--/*-----------------------------------------------------------------------*/


EXECUTE [dbo].[spSysCacheMonitor_Initialise];