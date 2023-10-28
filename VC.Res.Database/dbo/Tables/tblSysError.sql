CREATE TABLE [dbo].[tblSysError]
(
    [tblSysError_id] INT NOT NULL  IDENTITY, 
    [tblSysError_logger] NVARCHAR(100) NULL, 
    [tblSysError_level] NVARCHAR(100) NULL, 
    [tblSysErorr_priority] INT NOT NULL DEFAULT 3, 
    [tblSysError_class] NVARCHAR(MAX) NULL, 
    [tblSysError_method] NVARCHAR(MAX) NULL, 
    [tblSysError_parameters] NVARCHAR(MAX) NULL, 
    [tblSysError_message] NVARCHAR(MAX) NULL, 
    [tblSysError_stackTrace] NVARCHAR(MAX) NULL, 
    [tblSysError_innerEx] NVARCHAR(MAX) NULL, 
    [tblSysError_other] NVARCHAR(MAX) NULL, 
    [tblSysError_occurredUTC] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [tblSysError_dismissedUTC] DATETIME NULL, 
    [tblSysError_dismissedBy] NVARCHAR(200), 
    CONSTRAINT [PK_tblSysError] PRIMARY KEY ([tblSysError_id])
)