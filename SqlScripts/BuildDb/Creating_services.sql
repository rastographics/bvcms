CREATE SERVICE [UpdateAttendStrService]
AUTHORIZATION [dbo]
ON QUEUE [dbo].[UpdateAttendStrQueue]
(
[UpdateAttendStrContract]
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
