ALTER TABLE [dbo].[Contactees] ADD CONSTRAINT [PK_Contactees] PRIMARY KEY CLUSTERED  ([ContactId], [PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
