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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
