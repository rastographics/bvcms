ALTER TABLE [dbo].[ManagedGiving] ADD CONSTRAINT [FK_ManagedGiving_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
