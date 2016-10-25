ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
