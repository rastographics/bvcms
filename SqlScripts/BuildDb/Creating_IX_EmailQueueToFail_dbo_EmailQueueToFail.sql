CREATE NONCLUSTERED INDEX [IX_EmailQueueToFail] ON [dbo].[EmailQueueToFail] ([time])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
