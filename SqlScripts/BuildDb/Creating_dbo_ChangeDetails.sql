CREATE TABLE [dbo].[ChangeDetails]
(
[Id] [int] NOT NULL,
[Field] [nvarchar] (50) NOT NULL,
[Before] [nvarchar] (max) NULL,
[After] [nvarchar] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
