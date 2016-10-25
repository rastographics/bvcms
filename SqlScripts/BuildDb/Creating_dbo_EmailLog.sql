CREATE TABLE [dbo].[EmailLog]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[fromaddr] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[toaddr] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[time] [datetime] NULL,
[subject] [nvarchar] (180) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
