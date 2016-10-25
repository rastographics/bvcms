ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK  ADD CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleStatusTypes] FOREIGN KEY ([BundleStatusId]) REFERENCES [lookup].[BundleStatusTypes] ([Id])
GO
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK  ADD CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleHeaderTypes] FOREIGN KEY ([BundleHeaderTypeId]) REFERENCES [lookup].[BundleHeaderTypes] ([Id])
GO
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK  ADD CONSTRAINT [BundleHeaders__Fund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
