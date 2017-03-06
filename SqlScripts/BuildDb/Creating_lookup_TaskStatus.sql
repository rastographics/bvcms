CREATE TABLE [lookup].[TaskStatus]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (50) NULL,
[Description] [nvarchar] (100) NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
