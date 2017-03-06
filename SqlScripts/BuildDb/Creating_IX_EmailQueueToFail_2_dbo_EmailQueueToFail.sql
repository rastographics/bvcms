CREATE NONCLUSTERED INDEX [IX_EmailQueueToFail_2] ON [dbo].[EmailQueueToFail] ([timestamp]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
