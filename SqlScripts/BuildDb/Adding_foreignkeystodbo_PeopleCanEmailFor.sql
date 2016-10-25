ALTER TABLE [dbo].[PeopleCanEmailFor] ADD CONSTRAINT [OnBehalfOfPeople__PersonCanEmail] FOREIGN KEY ([CanEmail]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[PeopleCanEmailFor] ADD CONSTRAINT [PersonsCanEmail__OnBehalfOfPerson] FOREIGN KEY ([OnBehalfOf]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
