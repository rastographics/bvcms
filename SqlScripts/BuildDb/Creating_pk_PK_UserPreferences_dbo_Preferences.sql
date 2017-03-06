ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [PK_UserPreferences] PRIMARY KEY CLUSTERED  ([UserId], [Preference]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
