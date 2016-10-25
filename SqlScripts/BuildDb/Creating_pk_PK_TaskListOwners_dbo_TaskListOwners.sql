ALTER TABLE [dbo].[TaskListOwners] ADD CONSTRAINT [PK_TaskListOwners] PRIMARY KEY CLUSTERED  ([TaskListId], [PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
