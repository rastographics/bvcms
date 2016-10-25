CREATE NONCLUSTERED INDEX [IX_Contact] ON [dbo].[Contact] ([LimitToRole])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
