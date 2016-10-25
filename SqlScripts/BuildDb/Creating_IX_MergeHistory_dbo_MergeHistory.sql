CREATE NONCLUSTERED INDEX [IX_MergeHistory] ON [dbo].[MergeHistory] ([ToId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
