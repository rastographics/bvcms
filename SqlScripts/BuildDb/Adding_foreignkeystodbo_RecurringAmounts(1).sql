ALTER TABLE [dbo].[RecurringAmounts] ADD CONSTRAINT [FK_RecurringAmounts_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
