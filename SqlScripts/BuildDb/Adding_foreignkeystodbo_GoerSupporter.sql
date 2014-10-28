ALTER TABLE [dbo].[GoerSupporter] ADD CONSTRAINT [FK_Supporters__Goer] FOREIGN KEY ([GoerId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[GoerSupporter] ADD CONSTRAINT [FK_Goers__Supporter] FOREIGN KEY ([SupporterId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
