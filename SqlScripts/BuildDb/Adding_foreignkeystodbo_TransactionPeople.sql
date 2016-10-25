ALTER TABLE [dbo].[TransactionPeople] ADD CONSTRAINT [FK_TransactionPeople_Person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[TransactionPeople] ADD CONSTRAINT [FK_TransactionPeople_Transaction] FOREIGN KEY ([Id]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
