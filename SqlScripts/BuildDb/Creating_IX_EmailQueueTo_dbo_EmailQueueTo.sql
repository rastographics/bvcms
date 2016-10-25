CREATE NONCLUSTERED INDEX [IX_EmailQueueTo] ON [dbo].[EmailQueueTo] ([guid])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
