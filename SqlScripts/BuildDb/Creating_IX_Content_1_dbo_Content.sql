CREATE NONCLUSTERED INDEX [IX_Content_1] ON [dbo].[Content] ([ArchivedFromId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
