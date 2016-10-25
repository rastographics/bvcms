CREATE NONCLUSTERED INDEX [IX_ActivityLog_UserId] ON [dbo].[ActivityLog] ([UserId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
