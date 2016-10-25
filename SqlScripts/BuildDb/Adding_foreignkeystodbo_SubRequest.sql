ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubRequests__Attend] FOREIGN KEY ([AttendId]) REFERENCES [dbo].[Attend] ([AttendId])
GO
ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubRequests__Requestor] FOREIGN KEY ([RequestorId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubResponses__Substitute] FOREIGN KEY ([SubstituteId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
