CREATE TABLE [dbo].[ResourceCategory]
(
[ResourceCategoryId] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (50) NOT NULL,
[ResourceTypeId] [int] NOT NULL,
[DisplayOrder] [int] NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
