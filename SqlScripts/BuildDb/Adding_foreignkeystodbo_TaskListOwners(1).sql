ALTER TABLE [dbo].[TaskListOwners] ADD CONSTRAINT [FK_TaskListOwners_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
