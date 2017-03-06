CREATE TABLE [dbo].[Audits]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Action] [nvarchar] (20) NOT NULL,
[TableName] [nvarchar] (100) NOT NULL,
[TableKey] [int] NULL,
[UserName] [nvarchar] (50) NOT NULL,
[AuditDate] [smalldatetime] NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
