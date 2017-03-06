CREATE TABLE [dbo].[OrgMemMemTags]
(
[OrgId] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[MemberTagId] [int] NOT NULL,
[Number] [int] NULL,
[IsLeader] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
