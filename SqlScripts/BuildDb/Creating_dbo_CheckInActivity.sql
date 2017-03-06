CREATE TABLE [dbo].[CheckInActivity]
(
[Id] [int] NOT NULL,
[Activity] [nvarchar] (50) NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
