ALTER TABLE [dbo].[Contactors] ADD CONSTRAINT [contactsMakers__contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[Contactors] ADD CONSTRAINT [contactsMade__person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
