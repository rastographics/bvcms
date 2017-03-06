CREATE NONCLUSTERED INDEX [IDX_AttendAttendanceFlagMeetingDate] ON [dbo].[Attend] ([AttendanceFlag], [MeetingDate]) INCLUDE ([MeetingId], [PeopleId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
