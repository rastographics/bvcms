ALTER TABLE [dbo].[MobileAppDevices] ADD CONSTRAINT [FK_MobileAppDevices_Users] FOREIGN KEY ([userID]) REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[MobileAppDevices] ADD CONSTRAINT [FK_MobileAppDevices_People] FOREIGN KEY ([peopleID]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
