ALTER TABLE [dbo].[TagShare] ADD CONSTRAINT [FK_TagShare_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[TagShare] ADD CONSTRAINT [FK_TagShare_Tag] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
