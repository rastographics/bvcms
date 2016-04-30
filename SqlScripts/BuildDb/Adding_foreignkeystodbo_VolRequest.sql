ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolRequests__Requestor] FOREIGN KEY ([RequestorId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolResponses__Volunteer] FOREIGN KEY ([VolunteerId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolRequests__Meeting] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meetings] ([MeetingId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
