ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [FK_Transaction_People] FOREIGN KEY ([LoginPeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [Transactions__OriginalTransaction] FOREIGN KEY ([OriginalId]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
