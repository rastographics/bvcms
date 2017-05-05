CREATE TABLE [dbo].[Setting]
(
[Id] [nvarchar] (50) NOT NULL,
[Setting] [nvarchar] (max) NULL,
[System] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
