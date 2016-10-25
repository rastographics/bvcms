CREATE VIEW [dbo].[OrgSchedules2] AS

	SELECT 
		os.OrganizationId, 
		os.SchedTime, 
		os.SchedDay 
	FROM dbo.OrgSchedule os

	UNION

	SELECT 
		OrganizationId,
		CONVERT(DATETIME, [Time]),
		[DayOfWeek]
	FROM VolunteerTimes
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
