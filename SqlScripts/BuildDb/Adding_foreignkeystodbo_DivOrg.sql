ALTER TABLE [dbo].[DivOrg] ADD CONSTRAINT [FK_DivOrg_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
GO
ALTER TABLE [dbo].[DivOrg] ADD CONSTRAINT [FK_DivOrg_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
