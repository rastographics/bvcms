ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionFund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
GO
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionType] FOREIGN KEY ([ContributionTypeId]) REFERENCES [lookup].[ContributionType] ([Id])
GO
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionStatus] FOREIGN KEY ([ContributionStatusId]) REFERENCES [lookup].[ContributionStatus] ([Id])
GO
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ExtraData] FOREIGN KEY ([ExtraDataId]) REFERENCES [dbo].[ExtraData] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
