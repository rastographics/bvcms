ALTER TABLE [dbo].[TransactionPeople] ADD CONSTRAINT [FK_TransactionPeople_Person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[TransactionPeople] ADD CONSTRAINT [FK_TransactionPeople_Transaction] FOREIGN KEY ([Id]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
