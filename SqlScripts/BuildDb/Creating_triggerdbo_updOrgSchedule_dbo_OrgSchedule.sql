-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[updOrgSchedule] 
   ON  [dbo].[OrgSchedule] 
   AFTER UPDATE, INSERT
AS 
BEGIN
	SET NOCOUNT ON;

	IF UPDATE(SchedDay) OR UPDATE(SchedTime)
		UPDATE dbo.OrgSchedule
		SET ScheduleId = dbo.ScheduleId(SchedDay, SchedTime),
		MeetingTime = dbo.GetScheduleTime(SchedDay, SchedTime)
		WHERE Id IN (SELECT Id FROM INSERTED)
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
