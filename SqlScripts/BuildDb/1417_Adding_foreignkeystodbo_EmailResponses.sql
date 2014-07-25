ALTER TABLE [dbo].[EmailResponses] ADD CONSTRAINT [FK_EmailResponses_EmailQueue] FOREIGN KEY ([EmailQueueId]) REFERENCES [dbo].[EmailQueue] ([Id])
ALTER TABLE [dbo].[EmailResponses] ADD CONSTRAINT [FK_EmailResponses_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
