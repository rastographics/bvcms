ALTER TABLE [dbo].[OrgSchedule] ADD CONSTRAINT [PK_OrgSchedule] PRIMARY KEY CLUSTERED  ([OrganizationId], [Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
