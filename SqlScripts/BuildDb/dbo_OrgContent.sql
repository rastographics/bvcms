CREATE TABLE [dbo].[OrgContent]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[OrgId] [int] NULL,
[AllowInactive] [bit] NULL,
[PublicView] [bit] NULL,
[ImageId] [int] NULL,
[Landing] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
