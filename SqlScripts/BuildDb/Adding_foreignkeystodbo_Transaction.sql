ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [FK_Transaction_People] FOREIGN KEY ([LoginPeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [Transactions__OriginalTransaction] FOREIGN KEY ([OriginalId]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
