CREATE TABLE [dbo].[DownlineLeaders]
(
[CategoryId] [int] NULL,
[PeopleId] [int] NULL,
[Name] [nvarchar] (100) NULL,
[Cnt] [int] NULL,
[Levels] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
