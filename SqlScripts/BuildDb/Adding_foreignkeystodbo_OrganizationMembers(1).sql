ALTER TABLE [dbo].[OrganizationMembers] ADD CONSTRAINT [FK_OrganizationMembers_RegistrationData] FOREIGN KEY ([RegistrationDataId]) REFERENCES [dbo].[RegistrationData] ([Id])
ALTER TABLE [dbo].[OrganizationMembers] ADD CONSTRAINT [FK_OrganizationMembers_Transaction] FOREIGN KEY ([TranId]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
