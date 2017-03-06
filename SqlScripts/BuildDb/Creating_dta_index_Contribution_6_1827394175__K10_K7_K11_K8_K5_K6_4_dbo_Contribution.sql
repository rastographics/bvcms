CREATE NONCLUSTERED INDEX [_dta_index_Contribution_6_1827394175__K10_K7_K11_K8_K5_K6_4] ON [dbo].[Contribution] ([ContributionStatusId], [ContributionDate], [PledgeFlag], [ContributionAmount], [ContributionTypeId], [PeopleId]) INCLUDE ([FundId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
