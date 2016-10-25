ALTER TABLE [dbo].[ResourceOrganizationType] ADD CONSTRAINT [FK_ResourceOrganizationType_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
GO
ALTER TABLE [dbo].[ResourceOrganizationType] ADD CONSTRAINT [FK_ResourceOrganizationType_OrganizationType] FOREIGN KEY ([OrganizationTypeId]) REFERENCES [lookup].[OrganizationType] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
