ALTER TABLE [dbo].[Tag] ADD CONSTRAINT [TagsOwned__PersonOwner] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
