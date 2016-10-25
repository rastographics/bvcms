CREATE NONCLUSTERED INDEX [IX_ContentKeyWords] ON [dbo].[ContentKeyWords] ([Word])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
