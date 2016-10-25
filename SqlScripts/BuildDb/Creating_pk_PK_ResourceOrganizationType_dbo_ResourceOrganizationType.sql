ALTER TABLE [dbo].[ResourceOrganizationType] ADD CONSTRAINT [PK_ResourceOrganizationType] PRIMARY KEY CLUSTERED  ([ResourceId], [OrganizationTypeId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
