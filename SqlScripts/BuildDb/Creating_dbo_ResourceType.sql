CREATE TABLE [dbo].[ResourceType]
(
[ResourceTypeId] [int] NOT NULL IDENTITY(1, 1),
[Name] [varchar] (50) NOT NULL,
[DisplayOrder] [int] NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
