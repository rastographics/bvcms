CREATE NONCLUSTERED INDEX [IX_ActivityLog_Dt] ON [dbo].[ActivityLog] ([ActivityDate] DESC)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
