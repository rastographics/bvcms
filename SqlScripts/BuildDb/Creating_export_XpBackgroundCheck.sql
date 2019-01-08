CREATE VIEW [export].[XpBackgroundCheck] AS 
SELECT
	PeopleId ,
    [Status] = s.[Description] ,
    ProcessedDate ,
    Comments ,
    MVRStatusId ,
    MVRProcessedDate 
FROM dbo.Volunteer v
LEFT JOIN lookup.VolApplicationStatus s ON s.Id = v.StatusId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
