 
CREATE VIEW [dbo].[TaskSearch] AS  
SELECT 
    Created = CONVERT(DATE, t.CreatedOn) , 
    Status = ts.Description , 
    Due = CONVERT(DATE, t.Due) , 
    Completed = CONVERT(DATE, t.CompletedOn) , 
    t.Archive , 
    t.Notes , 
    t.[Description] , 
    t.ForceCompleteWContact , 
    t.DeclineReason , 
    t.LimitToRole , 
	Originator = og.Name, 
	[Owner] = op.Name, 
	Delegate = de.Name, 
	About = ab.Name, 
 
	Originator2 = og.Name2, 
	Owner2 = op.Name2, 
	Delegate2 = de.Name2, 
	About2 = ab.Name2, 
 
	t.Id , 
    t.StatusId , 
    t.OwnerId , 
    t.CoOwnerId , 
    t.OrginatorId , 
    t.WhoId , 
    t.SourceContactId , 
    t.CompletedContactId 
FROM dbo.Task t 
JOIN lookup.TaskStatus ts ON ts.Id = t.StatusId 
JOIN dbo.People op ON op.PeopleId = t.OwnerId 
LEFT JOIN dbo.People de ON de.PeopleId = t.CoOwnerId 
LEFT JOIN dbo.People ab ON ab.PeopleId = t.WhoId 
LEFT JOIN dbo.People og ON og.PeopleId = t.OrginatorId 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
