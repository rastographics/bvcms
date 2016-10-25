CREATE TABLE [dbo].[EmailToText]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Carrier] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[domain] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
