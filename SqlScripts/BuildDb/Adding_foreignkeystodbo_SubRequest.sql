ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubRequests__Attend] FOREIGN KEY ([AttendId]) REFERENCES [dbo].[Attend] ([AttendId])
ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubRequests__Requestor] FOREIGN KEY ([RequestorId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubResponses__Substitute] FOREIGN KEY ([SubstituteId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
