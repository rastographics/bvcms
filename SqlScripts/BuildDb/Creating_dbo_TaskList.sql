CREATE TABLE [dbo].[TaskList]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[CreatedBy] [int] NULL,
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
