CREATE TABLE [dbo].[TaskList]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[CreatedBy] [int] NULL,
[Name] [nvarchar] (50) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
