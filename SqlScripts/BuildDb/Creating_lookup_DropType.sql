CREATE TABLE [lookup].[DropType]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (20) NULL,
[Description] [nvarchar] (100) NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
