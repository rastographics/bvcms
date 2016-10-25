ALTER TABLE [dbo].[RecReg] ADD CONSTRAINT [FK_RecReg_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
