ALTER TABLE [dbo].[BackgroundChecks] ADD CONSTRAINT [FK_BackgroundChecks_People] FOREIGN KEY ([PeopleID]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[BackgroundChecks] ADD CONSTRAINT [People__User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
