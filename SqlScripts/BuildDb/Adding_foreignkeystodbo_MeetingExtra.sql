ALTER TABLE [dbo].[MeetingExtra] ADD CONSTRAINT [FK_MeetingExtra_Meetings] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meetings] ([MeetingId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
