CREATE TABLE [dbo].[PrintJob]
(
[Id] [nvarchar] (50) NOT NULL,
[Stamp] [datetime] NOT NULL,
[Data] [nvarchar] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
