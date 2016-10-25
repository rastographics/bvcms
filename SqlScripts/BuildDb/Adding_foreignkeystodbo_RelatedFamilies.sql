ALTER TABLE [dbo].[RelatedFamilies] WITH NOCHECK  ADD CONSTRAINT [RelatedFamilies1__RelatedFamily1] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
GO
ALTER TABLE [dbo].[RelatedFamilies] WITH NOCHECK  ADD CONSTRAINT [RelatedFamilies2__RelatedFamily2] FOREIGN KEY ([RelatedFamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
