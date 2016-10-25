CREATE TABLE [dbo].[Program]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[RptGroup] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StartHoursOffset] [real] NULL,
[EndHoursOffset] [real] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
