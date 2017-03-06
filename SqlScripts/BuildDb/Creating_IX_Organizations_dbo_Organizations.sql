CREATE NONCLUSTERED INDEX [IX_Organizations] ON [dbo].[Organizations] ([DivisionId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
