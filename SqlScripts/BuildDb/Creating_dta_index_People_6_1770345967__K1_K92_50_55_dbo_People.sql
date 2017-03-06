CREATE NONCLUSTERED INDEX [_dta_index_People_6_1770345967__K1_K92_50_55] ON [dbo].[People] ([PeopleId], [ContributionOptionsId]) INCLUDE ([FirstName], [NickName]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
