CREATE TABLE [dbo].[DownlineLeaders]
(
[CategoryId] [int] NULL,
[PeopleId] [int] NULL,
[Name] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Cnt] [int] NULL,
[Levels] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
