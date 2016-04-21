CREATE TABLE [dbo].[OrgMemMemTags]
(
[OrgId] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[MemberTagId] [int] NOT NULL,
[IsLeader] [bit] NULL,
[Number] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
