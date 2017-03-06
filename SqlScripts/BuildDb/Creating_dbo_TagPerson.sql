CREATE TABLE [dbo].[TagPerson]
(
[Id] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[DateCreated] [datetime] NULL CONSTRAINT [DF_TagPerson_DateCreated] DEFAULT (getdate())
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
