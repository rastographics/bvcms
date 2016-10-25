CREATE TABLE [lookup].[TaskStatus]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Description] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
