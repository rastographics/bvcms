CREATE TABLE [dbo].[RssFeed]
(
[Url] [nvarchar] (150) NOT NULL,
[Data] [nvarchar] (max) NULL,
[ETag] [nvarchar] (150) NULL,
[LastModified] [datetime] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
