CREATE NONCLUSTERED INDEX [IX_ContentKeyWords] ON [dbo].[ContentKeyWords] ([Word]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
