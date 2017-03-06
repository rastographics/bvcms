CREATE TABLE [dbo].[OrgContent]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[OrgId] [int] NULL,
[AllowInactive] [bit] NULL,
[PublicView] [bit] NULL,
[ImageId] [int] NULL,
[Landing] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
