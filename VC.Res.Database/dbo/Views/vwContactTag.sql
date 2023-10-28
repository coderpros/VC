CREATE VIEW [dbo].[vwContactTag]
	AS
    SELECT tblContactTag.tblContactTag_id,
            tblContactTag.tblContact_id,
            tblContactTag.tblTag_id,
            tblTag.tblTag_type,
            tblTag.tblTag_name,
            tblTag.tblTag_desc,
            tblTag.tblTag_icon,
            tblContactTag.tblContactTag_order 
    FROM tblContactTag
        INNER JOIN tblTag ON tblContactTag.tblTag_id = tblTag.tblTag_id 
