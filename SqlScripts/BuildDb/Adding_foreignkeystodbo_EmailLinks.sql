ALTER TABLE [dbo].[EmailLinks] ADD CONSTRAINT [FK_EmailLinks_EmailQueue] FOREIGN KEY ([EmailID]) REFERENCES [dbo].[EmailQueue] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
