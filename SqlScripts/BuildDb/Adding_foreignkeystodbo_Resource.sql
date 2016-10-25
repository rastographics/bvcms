ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_Division] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Division] ([Id])
GO
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_ResourceCategory] FOREIGN KEY ([ResourceCategoryId]) REFERENCES [dbo].[ResourceCategory] ([ResourceCategoryId])
GO
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_ResourceType] FOREIGN KEY ([ResourceTypeId]) REFERENCES [dbo].[ResourceType] ([ResourceTypeId])
GO
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
GO
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_Resource_OrganizationType] FOREIGN KEY ([OrganizationTypeId]) REFERENCES [lookup].[OrganizationType] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
