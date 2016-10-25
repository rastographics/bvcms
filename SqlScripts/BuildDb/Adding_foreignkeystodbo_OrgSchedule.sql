ALTER TABLE [dbo].[OrgSchedule] ADD CONSTRAINT [FK_OrgSchedule_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
