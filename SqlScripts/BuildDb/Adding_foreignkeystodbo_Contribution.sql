ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionFund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ExtraData] FOREIGN KEY ([ExtraDataId]) REFERENCES [dbo].[ExtraData] ([Id])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionType] FOREIGN KEY ([ContributionTypeId]) REFERENCES [lookup].[ContributionType] ([Id])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionStatus] FOREIGN KEY ([ContributionStatusId]) REFERENCES [lookup].[ContributionStatus] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
