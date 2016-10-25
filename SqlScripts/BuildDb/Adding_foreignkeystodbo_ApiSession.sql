ALTER TABLE [dbo].[ApiSession] ADD CONSTRAINT [FK_Users_ApiSession] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
