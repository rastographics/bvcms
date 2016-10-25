ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Division] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Division] ([Id])
GO
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_ORGANIZATIONS_TBL_OrganizationStatus] FOREIGN KEY ([OrganizationStatusId]) REFERENCES [lookup].[OrganizationStatus] ([Id])
GO
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_ORGANIZATIONS_TBL_EntryPoint] FOREIGN KEY ([EntryPointId]) REFERENCES [lookup].[EntryPoint] ([Id])
GO
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
GO
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Gender] FOREIGN KEY ([GenderId]) REFERENCES [lookup].[Gender] ([Id])
GO
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_OrganizationType] FOREIGN KEY ([OrganizationTypeId]) REFERENCES [lookup].[OrganizationType] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
