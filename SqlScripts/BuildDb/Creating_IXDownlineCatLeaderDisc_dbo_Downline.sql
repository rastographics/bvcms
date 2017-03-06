CREATE NONCLUSTERED INDEX [IXDownlineCatLeaderDisc] ON [dbo].[Downline] ([CategoryId], [DownlineId], [DiscipleId]) INCLUDE ([Generation]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
