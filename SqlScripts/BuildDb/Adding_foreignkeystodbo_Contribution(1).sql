ALTER TABLE [dbo].[Contribution] ADD CONSTRAINT [FK_Contribution_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
