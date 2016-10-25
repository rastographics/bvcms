ALTER TABLE [dbo].[BundleDetail] WITH NOCHECK  ADD CONSTRAINT [BUNDLE_DETAIL_BUNDLE_FK] FOREIGN KEY ([BundleHeaderId]) REFERENCES [dbo].[BundleHeader] ([BundleHeaderId])
GO
ALTER TABLE [dbo].[BundleDetail] WITH NOCHECK  ADD CONSTRAINT [BUNDLE_DETAIL_CONTR_FK] FOREIGN KEY ([ContributionId]) REFERENCES [dbo].[Contribution] ([ContributionId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
