ALTER TABLE [dbo].[PeopleExtra] ADD CONSTRAINT [PK_PeopleExtra_1] PRIMARY KEY CLUSTERED  ([PeopleId], [Field], [Instance]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
