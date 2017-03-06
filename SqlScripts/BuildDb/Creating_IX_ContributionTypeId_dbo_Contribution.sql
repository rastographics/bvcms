CREATE NONCLUSTERED INDEX [IX_ContributionTypeId] ON [dbo].[Contribution] ([ContributionTypeId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
