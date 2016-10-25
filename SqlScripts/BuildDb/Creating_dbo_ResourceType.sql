CREATE TABLE [dbo].[ResourceType]
(
[ResourceTypeId] [int] NOT NULL IDENTITY(1, 1),
[Name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[DisplayOrder] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
