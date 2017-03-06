ALTER TABLE [dbo].[MobileAppPushRegistrations] ADD CONSTRAINT [PK_MobileAppPushRegistrations] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
