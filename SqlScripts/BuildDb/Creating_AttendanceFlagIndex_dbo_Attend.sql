CREATE NONCLUSTERED INDEX [AttendanceFlagIndex] ON [dbo].[Attend] ([AttendanceFlag]) INCLUDE ([MeetingDate], [OrganizationId], [PeopleId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
