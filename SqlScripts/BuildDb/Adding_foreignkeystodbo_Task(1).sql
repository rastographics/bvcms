ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksAssigned__SourceContact] FOREIGN KEY ([SourceContactId]) REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksCompleted__CompletedContact] FOREIGN KEY ([CompletedContactId]) REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [Tasks__Owner] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksAboutPerson__AboutWho] FOREIGN KEY ([WhoId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksCoOwned__CoOwner] FOREIGN KEY ([CoOwnerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
