CREATE TABLE [dbo].[ResourceCategory]
(
[ResourceCategoryId] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[ResourceTypeId] [int] NOT NULL,
[DisplayOrder] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
