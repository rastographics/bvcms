ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [FK_UserPreferences_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
