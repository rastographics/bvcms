ALTER TABLE [dbo].[ContentKeyWords] WITH NOCHECK  ADD CONSTRAINT [FK_ContentKeyWords_Content] FOREIGN KEY ([Id]) REFERENCES [dbo].[Content] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
