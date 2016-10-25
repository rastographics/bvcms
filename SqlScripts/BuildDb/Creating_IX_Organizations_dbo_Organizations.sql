CREATE NONCLUSTERED INDEX [IX_Organizations] ON [dbo].[Organizations] ([DivisionId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
