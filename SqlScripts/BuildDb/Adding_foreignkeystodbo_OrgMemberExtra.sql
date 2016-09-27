ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [FK_OrgMemberExtra_OrganizationMembers] FOREIGN KEY ([OrganizationId], [PeopleId]) REFERENCES [dbo].[OrganizationMembers] ([OrganizationId], [PeopleId])
ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [FK_OrgMemberExtra_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [FK_OrgMemberExtra_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
