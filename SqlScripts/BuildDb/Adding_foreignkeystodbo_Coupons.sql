ALTER TABLE [dbo].[Coupons] WITH NOCHECK  ADD CONSTRAINT [FK_Coupons_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
GO
ALTER TABLE [dbo].[Coupons] WITH NOCHECK  ADD CONSTRAINT [FK_Coupons_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
