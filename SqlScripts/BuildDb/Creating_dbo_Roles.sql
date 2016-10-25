CREATE TABLE [dbo].[Roles]
(
[RoleName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[RoleId] [int] NOT NULL IDENTITY(1, 1),
[hardwired] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
