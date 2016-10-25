ALTER TABLE [dbo].[OrganizationMembers] ADD CONSTRAINT [FK_OrganizationMembers_Transaction] FOREIGN KEY ([TranId]) REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[OrganizationMembers] ADD CONSTRAINT [FK_OrganizationMembers_RegistrationData] FOREIGN KEY ([RegistrationDataId]) REFERENCES [dbo].[RegistrationData] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
