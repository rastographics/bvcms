ALTER TABLE [dbo].[RecurringAmounts] WITH NOCHECK  ADD CONSTRAINT [FK_RecurringAmounts_ContributionFund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
