ALTER TABLE [dbo].[EmailOptOut] ADD CONSTRAINT [FK_EmailOptOut_People] FOREIGN KEY ([ToPeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
