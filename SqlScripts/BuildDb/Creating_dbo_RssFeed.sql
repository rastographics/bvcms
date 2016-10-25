CREATE TABLE [dbo].[RssFeed]
(
[Url] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Data] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ETag] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LastModified] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
