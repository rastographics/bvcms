CREATE TABLE [dbo].[CardIdentifiers]
(
[Id] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[PeopleId] [int] NULL,
[CreatedOn] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
