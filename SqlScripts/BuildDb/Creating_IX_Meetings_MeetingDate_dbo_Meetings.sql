CREATE NONCLUSTERED INDEX [IX_Meetings_MeetingDate] ON [dbo].[Meetings] ([MeetingDate] DESC)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
