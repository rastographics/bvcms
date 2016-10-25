ALTER TABLE [dbo].[PeopleExtra] ADD CONSTRAINT [FK_PeopleExtra_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
