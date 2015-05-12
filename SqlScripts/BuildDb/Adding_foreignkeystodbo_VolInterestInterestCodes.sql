ALTER TABLE [dbo].[VolInterestInterestCodes] ADD CONSTRAINT [FK_VolInterestInterestCodes_VolInterestCodes] FOREIGN KEY ([InterestCodeId]) REFERENCES [dbo].[VolInterestCodes] ([Id])
ALTER TABLE [dbo].[VolInterestInterestCodes] ADD CONSTRAINT [FK_VolInterestInterestCodes_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
