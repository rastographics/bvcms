ALTER TABLE [dbo].[OrganizationExtra] ADD CONSTRAINT [PK_OrganizationExtra] PRIMARY KEY CLUSTERED  ([OrganizationId], [Field]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
