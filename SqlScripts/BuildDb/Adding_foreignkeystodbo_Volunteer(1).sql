ALTER TABLE [dbo].[Volunteer] ADD CONSTRAINT [FK_Volunteer_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
