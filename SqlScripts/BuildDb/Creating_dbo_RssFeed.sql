CREATE TABLE [dbo].[RssFeed]
(
[Url] [nvarchar] (150) NOT NULL,
[Data] [nvarchar] (max) NULL,
[ETag] [nvarchar] (150) NULL,
[LastModified] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
