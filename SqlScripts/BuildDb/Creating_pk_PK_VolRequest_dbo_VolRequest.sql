ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [PK_VolRequest] PRIMARY KEY CLUSTERED  ([MeetingId], [RequestorId], [Requested], [VolunteerId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
