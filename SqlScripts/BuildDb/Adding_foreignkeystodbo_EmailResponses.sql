ALTER TABLE [dbo].[EmailResponses] ADD CONSTRAINT [FK_EmailResponses_EmailQueue] FOREIGN KEY ([EmailQueueId]) REFERENCES [dbo].[EmailQueue] ([Id])
GO
ALTER TABLE [dbo].[EmailResponses] ADD CONSTRAINT [FK_EmailResponses_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
