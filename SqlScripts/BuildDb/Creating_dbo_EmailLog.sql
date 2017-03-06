CREATE TABLE [dbo].[EmailLog]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[fromaddr] [nvarchar] (50) NULL,
[toaddr] [nvarchar] (150) NULL,
[time] [datetime] NULL,
[subject] [nvarchar] (180) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
