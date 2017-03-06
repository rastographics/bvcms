CREATE TABLE [dbo].[Roles]
(
[RoleName] [nvarchar] (50) NULL,
[RoleId] [int] NOT NULL IDENTITY(1, 1),
[hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
