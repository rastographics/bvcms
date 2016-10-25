ALTER TABLE [dbo].[OrgMemMemTags] ADD CONSTRAINT [FK_OrgMemMemTags_MemberTags] FOREIGN KEY ([MemberTagId]) REFERENCES [dbo].[MemberTags] ([Id])
GO
ALTER TABLE [dbo].[OrgMemMemTags] ADD CONSTRAINT [FK_OrgMemMemTags_OrganizationMembers] FOREIGN KEY ([OrgId], [PeopleId]) REFERENCES [dbo].[OrganizationMembers] ([OrganizationId], [PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
