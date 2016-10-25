CREATE TABLE [dbo].[ChangeDetails]
(
[Id] [int] NOT NULL,
[Field] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Before] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[After] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
