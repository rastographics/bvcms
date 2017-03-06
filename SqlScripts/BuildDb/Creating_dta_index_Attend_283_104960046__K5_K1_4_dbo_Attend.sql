CREATE NONCLUSTERED INDEX [_dta_index_Attend_283_104960046__K5_K1_4] ON [dbo].[Attend] ([AttendanceFlag], [PeopleId]) INCLUDE ([MeetingDate]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
