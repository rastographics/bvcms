CREATE NONCLUSTERED INDEX [IX_ActivityLog_DatumId] ON [dbo].[ActivityLog] ([DatumId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
