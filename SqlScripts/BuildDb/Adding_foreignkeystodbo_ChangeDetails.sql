ALTER TABLE [dbo].[ChangeDetails] WITH NOCHECK  ADD CONSTRAINT [FK_ChangeDetails_ChangeLog] FOREIGN KEY ([Id]) REFERENCES [dbo].[ChangeLog] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
