CREATE VIEW [dbo].[vwPropertyCollection]
	AS
    SELECT 
        tblPropertyCollection.tblPropertyCollection_id,
        tblPropertyCollection.tblProperty_id,
        tblPropertyCollection.tblCollection_id,
        tblCollection.tblCollection_name,
        tblCollection.tblCollection_desc
    FROM tblPropertyCollection
        INNER JOIN tblCollection ON tblPropertyCollection.tblCollection_id = tblCollection.tblCollection_id