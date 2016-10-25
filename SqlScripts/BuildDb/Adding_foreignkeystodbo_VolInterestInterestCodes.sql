ALTER TABLE [dbo].[VolInterestInterestCodes] ADD CONSTRAINT [FK_VolInterestInterestCodes_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[VolInterestInterestCodes] ADD CONSTRAINT [FK_VolInterestInterestCodes_VolInterestCodes] FOREIGN KEY ([InterestCodeId]) REFERENCES [dbo].[VolInterestCodes] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
