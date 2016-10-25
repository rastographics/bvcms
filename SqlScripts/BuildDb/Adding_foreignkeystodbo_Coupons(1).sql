ALTER TABLE [dbo].[Coupons] ADD CONSTRAINT [FK_Coupons_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[Coupons] ADD CONSTRAINT [FK_Coupons_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
