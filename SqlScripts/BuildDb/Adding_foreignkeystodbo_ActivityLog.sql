ALTER TABLE [dbo].[ActivityLog] ADD CONSTRAINT [FK_ActivityLog_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[ActivityLog] ADD CONSTRAINT [FK_ActivityLog_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[ActivityLog] ADD CONSTRAINT [FK_ActivityLog_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
