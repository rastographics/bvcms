ALTER TABLE [dbo].[OrgMemberExtra] ADD CONSTRAINT [PK_OrgMemberExtra] PRIMARY KEY CLUSTERED  ([OrganizationId], [PeopleId], [Field]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
