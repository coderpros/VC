CREATE TABLE [dbo].[tblPropertyCollection]
(
	[tblPropertyCollection_id] INT NOT NULL PRIMARY KEY, 
    [tblCollection_id] INT NOT NULL,
    [tblProperty_id] INT NOT NULL, 
    CONSTRAINT [FK_tblPropertyCollection_tblCollection] FOREIGN KEY ([tblCollection_id]) REFERENCES [tblCollection]([tblCollection_id]),
    CONSTRAINT [FK_tblPropertyCollection_tblProperty] FOREIGN KEY ([tblProperty_id]) REFERENCES [tblProperty]([tblProperty_id])
)
