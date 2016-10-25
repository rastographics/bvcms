ALTER TABLE [dbo].[Zips] WITH NOCHECK  ADD CONSTRAINT [FK_Zips_ResidentCode] FOREIGN KEY ([MetroMarginalCode]) REFERENCES [lookup].[ResidentCode] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
