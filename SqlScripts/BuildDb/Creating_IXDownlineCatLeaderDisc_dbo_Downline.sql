CREATE NONCLUSTERED INDEX [IXDownlineCatLeaderDisc] ON [dbo].[Downline] ([CategoryId], [DownlineId], [DiscipleId]) INCLUDE ([Generation])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
