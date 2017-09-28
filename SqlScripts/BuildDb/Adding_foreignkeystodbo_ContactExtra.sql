ALTER TABLE [dbo].[ContactExtra] ADD CONSTRAINT [FK_ContactExtra_Contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contact] ([ContactId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
