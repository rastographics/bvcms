CREATE NONCLUSTERED INDEX [IX_EmailResponses] ON [dbo].[EmailResponses] ([EmailQueueId], [PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
