CREATE TABLE [dbo].[OrgMemMemTags]
(
[OrgId] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[MemberTagId] [int] NOT NULL,
[Number] [int] NULL,
[IsLeader] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
