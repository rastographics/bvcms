ALTER TABLE [dbo].[PeopleCanEmailFor] ADD CONSTRAINT [OnBehalfOfPeople__PersonCanEmail] FOREIGN KEY ([CanEmail]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[PeopleCanEmailFor] ADD CONSTRAINT [PersonsCanEmail__OnBehalfOfPerson] FOREIGN KEY ([OnBehalfOf]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
