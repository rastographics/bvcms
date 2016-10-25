ALTER TABLE [dbo].[ResourceCategory] ADD CONSTRAINT [FK_ResourceCategory_ResourceType] FOREIGN KEY ([ResourceTypeId]) REFERENCES [dbo].[ResourceType] ([ResourceTypeId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
