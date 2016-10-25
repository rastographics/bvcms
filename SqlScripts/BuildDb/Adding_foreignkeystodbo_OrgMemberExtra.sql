ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [FK_OrgMemberExtra_OrganizationMembers] FOREIGN KEY ([OrganizationId], [PeopleId]) REFERENCES [dbo].[OrganizationMembers] ([OrganizationId], [PeopleId])
GO
ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [FK_OrgMemberExtra_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [FK_OrgMemberExtra_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
