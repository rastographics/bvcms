CREATE NONCLUSTERED INDEX [IX_ContributionTypeId] ON [dbo].[Contribution] ([ContributionTypeId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
