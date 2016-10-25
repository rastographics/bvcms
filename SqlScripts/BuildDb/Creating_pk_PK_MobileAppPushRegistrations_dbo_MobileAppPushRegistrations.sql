ALTER TABLE [dbo].[MobileAppPushRegistrations] ADD CONSTRAINT [PK_MobileAppPushRegistrations] PRIMARY KEY CLUSTERED  ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
