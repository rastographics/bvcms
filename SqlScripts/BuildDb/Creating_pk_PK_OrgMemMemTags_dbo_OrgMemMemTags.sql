ALTER TABLE [dbo].[OrgMemMemTags] ADD CONSTRAINT [PK_OrgMemMemTags] PRIMARY KEY CLUSTERED  ([OrgId], [PeopleId], [MemberTagId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
