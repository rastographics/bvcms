ALTER TABLE [dbo].[Contact] ADD CONSTRAINT [contactsHad__organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
