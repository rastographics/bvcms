ALTER TABLE [dbo].[OrganizationExtra] WITH NOCHECK  ADD CONSTRAINT [FK_OrganizationExtra_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
