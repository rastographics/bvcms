CREATE NONCLUSTERED INDEX [IX_Downline] ON [dbo].[Downline] ([CategoryId], [DownlineId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
