ALTER TABLE [dbo].[PrevOrgMemberExtra] ADD CONSTRAINT [PK_PrevOrgMemberExtra] PRIMARY KEY CLUSTERED  ([EnrollmentTranId], [OrganizationId], [PeopleId], [Field])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
