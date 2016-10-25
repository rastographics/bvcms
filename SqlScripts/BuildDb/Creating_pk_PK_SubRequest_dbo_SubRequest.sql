ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [PK_SubRequest] PRIMARY KEY CLUSTERED  ([AttendId], [RequestorId], [Requested], [SubstituteId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
