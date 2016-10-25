ALTER TABLE [dbo].[CardIdentifiers] ADD CONSTRAINT [FK_CardIdentifiers_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
