ALTER TABLE [dbo].[FamilyCheckinLock] ADD CONSTRAINT [FK_FamilyCheckinLock_FamilyCheckinLock1] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
