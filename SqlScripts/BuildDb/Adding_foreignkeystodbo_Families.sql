ALTER TABLE [dbo].[Families] ADD CONSTRAINT [ResCodeFamilies__ResidentCode] FOREIGN KEY ([ResCodeId]) REFERENCES [lookup].[ResidentCode] ([Id])
GO
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FamiliesHeaded__HeadOfHousehold] FOREIGN KEY ([HeadOfHouseholdId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FamiliesHeaded2__HeadOfHouseholdSpouse] FOREIGN KEY ([HeadOfHouseholdSpouseId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FK_Families_Picture] FOREIGN KEY ([PictureId]) REFERENCES [dbo].[Picture] ([PictureId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
