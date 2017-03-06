CREATE TABLE [dbo].[Program]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (50) NULL,
[RptGroup] [nvarchar] (200) NULL,
[StartHoursOffset] [real] NULL,
[EndHoursOffset] [real] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
