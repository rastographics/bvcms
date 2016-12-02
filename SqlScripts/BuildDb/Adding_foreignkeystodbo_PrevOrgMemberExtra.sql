ALTER TABLE [dbo].[PrevOrgMemberExtra] ADD CONSTRAINT [FK_PrevOrgMemberExtra_EnrollmentTransaction] FOREIGN KEY ([EnrollmentTranId]) REFERENCES [dbo].[EnrollmentTransaction] ([TransactionId])
GO
ALTER TABLE [dbo].[PrevOrgMemberExtra] ADD CONSTRAINT [FK_PrevOrgMemberExtra_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[PrevOrgMemberExtra] ADD CONSTRAINT [FK_PrevOrgMemberExtra_People]]] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
