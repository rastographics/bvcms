CREATE NONCLUSTERED INDEX [_dta_index_Contribution_6_1827394175__K10_K7_K11_K8_K5_K6_4] ON [dbo].[Contribution] ([ContributionStatusId], [ContributionDate], [PledgeFlag], [ContributionAmount], [ContributionTypeId], [PeopleId]) INCLUDE ([FundId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
