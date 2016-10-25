ALTER TABLE [dbo].[Contactors] ADD CONSTRAINT [PK_Contactors] PRIMARY KEY CLUSTERED  ([ContactId], [PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
