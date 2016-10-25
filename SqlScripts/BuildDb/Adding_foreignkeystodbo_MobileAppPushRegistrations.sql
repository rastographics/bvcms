ALTER TABLE [dbo].[MobileAppPushRegistrations] ADD CONSTRAINT [FK_MobileAppPushRegistrations_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
