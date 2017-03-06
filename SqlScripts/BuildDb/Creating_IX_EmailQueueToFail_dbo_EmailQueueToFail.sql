CREATE NONCLUSTERED INDEX [IX_EmailQueueToFail] ON [dbo].[EmailQueueToFail] ([time]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
