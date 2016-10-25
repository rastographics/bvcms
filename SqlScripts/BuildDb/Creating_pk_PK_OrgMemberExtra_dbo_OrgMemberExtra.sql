ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [PK_OrgMemberExtra] PRIMARY KEY CLUSTERED  ([OrganizationId], [PeopleId], [Field])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
