ALTER TABLE [dbo].[Organizations] WITH NOCHECK  ADD CONSTRAINT [ChildOrgs__ParentOrg] FOREIGN KEY ([ParentOrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
