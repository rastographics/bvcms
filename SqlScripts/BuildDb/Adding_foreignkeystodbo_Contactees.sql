ALTER TABLE [dbo].[Contactees] ADD CONSTRAINT [contactees__contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[Contactees] ADD CONSTRAINT [contactsHad__person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
