ALTER TABLE [dbo].[MeetingExtra] ADD CONSTRAINT [PK_MeetingExtra] PRIMARY KEY CLUSTERED  ([MeetingId], [Field])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
