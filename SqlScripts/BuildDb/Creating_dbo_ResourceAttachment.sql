CREATE TABLE [dbo].[ResourceAttachment]
(
[ResourceAttachmentId] [int] NOT NULL IDENTITY(1, 1),
[ResourceId] [int] NOT NULL,
[FilePath] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FileTypeId] [int] NULL,
[Name] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreationDate] [datetime] NULL,
[UpdateDate] [datetime] NULL,
[DisplayOrder] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
