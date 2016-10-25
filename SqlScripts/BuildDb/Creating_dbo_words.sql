CREATE TABLE [dbo].[words]
(
[word] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[n] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
