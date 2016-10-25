ALTER TABLE [dbo].[ResourceOrganization] ADD CONSTRAINT [FK_ResourceOrganization_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[ResourceOrganization] ADD CONSTRAINT [FK_ResourceOrganization_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
