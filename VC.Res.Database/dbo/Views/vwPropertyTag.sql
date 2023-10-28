CREATE VIEW [dbo].[vwPropertyTag]
	AS
    SELECT tblPropertyTag.tblPropertyTag_id,
            tblPropertyTag.tblProperty_id,
            tblPropertyTag.tblTag_id,
            tblTag.tblTag_type,
            tblTag.tblTag_name,
            tblTag.tblTag_desc,
            tblTag.tblTag_icon,
            tblPropertyTag.tblPropertyTag_category,
            tblPropertyTag.tblPropertyTag_desc, 
            tblPropertyTag.tblPropertyTag_order  
    FROM tblPropertyTag
        INNER JOIN tblTag ON tblPropertyTag.tblTag_id = tblTag.tblTag_id 