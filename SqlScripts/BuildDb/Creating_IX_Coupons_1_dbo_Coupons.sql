CREATE NONCLUSTERED INDEX [IX_Coupons_1] ON [dbo].[Coupons] ([DivId], [OrgId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
