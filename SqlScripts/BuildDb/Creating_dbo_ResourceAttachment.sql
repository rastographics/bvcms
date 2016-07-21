CREATE TABLE [dbo].[ResourceAttachment]
(
[ResourceAttachmentId] [int] NOT NULL IDENTITY(1, 1),
[ResourceId] [int] NOT NULL,
[FilePath] [nvarchar] (max) NULL,
[FileTypeId] [int] NULL,
[Name] [nvarchar] (100) NULL,
[CreationDate] [datetime] NULL,
[UpdateDate] [datetime] NULL,
[DisplayOrder] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
