ALTER TABLE [dbo].[TagPerson] WITH NOCHECK  ADD CONSTRAINT [Tags__Person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[TagPerson] WITH NOCHECK  ADD CONSTRAINT [PersonTags__Tag] FOREIGN KEY ([Id]) REFERENCES [dbo].[Tag] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
