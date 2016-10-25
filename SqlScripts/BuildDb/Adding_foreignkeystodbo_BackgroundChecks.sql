ALTER TABLE [dbo].[BackgroundChecks] ADD CONSTRAINT [People__User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[BackgroundChecks] ADD CONSTRAINT [FK_BackgroundChecks_People] FOREIGN KEY ([PeopleID]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
