CREATE TABLE [dbo].[CardIdentifiers]
(
[Id] [nvarchar] (80) NOT NULL,
[PeopleId] [int] NULL,
[CreatedOn] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
