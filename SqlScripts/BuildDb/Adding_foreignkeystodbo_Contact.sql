ALTER TABLE [dbo].[Contact] WITH NOCHECK  ADD CONSTRAINT [FK_Contacts_ContactTypes] FOREIGN KEY ([ContactTypeId]) REFERENCES [lookup].[ContactType] ([Id])
GO
ALTER TABLE [dbo].[Contact] WITH NOCHECK  ADD CONSTRAINT [FK_NewContacts_ContactReasons] FOREIGN KEY ([ContactReasonId]) REFERENCES [lookup].[ContactReason] ([Id])
GO
ALTER TABLE [dbo].[Contact] WITH NOCHECK  ADD CONSTRAINT [FK_Contacts_Ministries] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministries] ([MinistryId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
