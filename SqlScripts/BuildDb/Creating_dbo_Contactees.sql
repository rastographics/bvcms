CREATE TABLE [dbo].[Contactees]
(
[ContactId] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[ProfessionOfFaith] [bit] NULL,
[PrayedForPerson] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
