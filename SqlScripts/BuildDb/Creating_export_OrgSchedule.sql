CREATE VIEW [export].[OrgSchedule] AS 
SELECT 
	Id,
	OrganizationId ,
    SchedTime = FORMAT(SchedTime, 'h:mm tt'),
    SchedDay = SchedDay + 1 
FROM dbo.OrgSchedule
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
