CREATE NONCLUSTERED INDEX [IX_QueryStat] ON [dbo].[QueryStats] ([Runtime])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
