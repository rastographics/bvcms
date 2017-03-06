CREATE NONCLUSTERED INDEX [IX_ActivityLog_1] ON [dbo].[ActivityLog] ([Activity]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
