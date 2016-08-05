ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_Division] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_ResourceCategory] FOREIGN KEY ([ResourceCategoryId]) REFERENCES [dbo].[ResourceCategory] ([ResourceCategoryId])
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_ResourceType] FOREIGN KEY ([ResourceTypeId]) REFERENCES [dbo].[ResourceType] ([ResourceTypeId])
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_OrganizationType] FOREIGN KEY ([OrganizationTypeId]) REFERENCES [lookup].[OrganizationType] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
