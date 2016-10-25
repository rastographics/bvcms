ALTER TABLE [dbo].[FamilyExtra] ADD CONSTRAINT [FK_FamilyExtra_Family] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
