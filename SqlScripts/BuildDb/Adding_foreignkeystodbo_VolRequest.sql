ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolRequests__Meeting] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meetings] ([MeetingId])
GO
ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolRequests__Requestor] FOREIGN KEY ([RequestorId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolResponses__Volunteer] FOREIGN KEY ([VolunteerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
