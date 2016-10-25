ALTER TABLE [dbo].[Attend] ADD CONSTRAINT [FK_AttendWithAbsents_TBL_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[Attend] ADD CONSTRAINT [FK_AttendWithAbsents_TBL_MEETINGS_TBL] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meetings] ([MeetingId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
