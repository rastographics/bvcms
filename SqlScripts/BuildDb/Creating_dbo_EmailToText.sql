CREATE TABLE [dbo].[EmailToText]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Carrier] [nvarchar] (50) NULL,
[domain] [nvarchar] (50) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
