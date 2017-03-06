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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
