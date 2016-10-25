ALTER TABLE [dbo].[EmailQueueTo] ADD CONSTRAINT [FK_EmailQueueTo_EmailQueue] FOREIGN KEY ([Id]) REFERENCES [dbo].[EmailQueue] ([Id])
GO
ALTER TABLE [dbo].[EmailQueueTo] ADD CONSTRAINT [FK_EmailQueueTo_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
