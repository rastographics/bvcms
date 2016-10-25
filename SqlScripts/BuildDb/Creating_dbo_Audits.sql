CREATE TABLE [dbo].[Audits]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Action] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[TableName] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[TableKey] [int] NULL,
[UserName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AuditDate] [smalldatetime] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
