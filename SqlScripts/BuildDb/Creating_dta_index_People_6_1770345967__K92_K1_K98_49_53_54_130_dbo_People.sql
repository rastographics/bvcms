CREATE NONCLUSTERED INDEX [_dta_index_People_6_1770345967__K92_K1_K98_49_53_54_130] ON [dbo].[People] ([ContributionOptionsId], [PeopleId], [SpouseId]) INCLUDE ([LastName], [SuffixCode], [TitleCode]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
