

CREATE VIEW [dbo].[IncompleteTasks]
AS
	SELECT 
		t.CreatedOn,
		o.Name Owner,
		co.Name DelegatedTo,
		t.Description,
		ab.Name About,
		t.Notes,
		ts.Description Status,
		t.Id,
		t.OwnerId,
		t.CoOwnerId,
		t.WhoId,
		t.StatusId,
		t.SourceContactId
	FROM dbo.Task t
	JOIN lookup.TaskStatus ts ON ts.Id = t.StatusId
	JOIN dbo.People o ON o.PeopleId = t.OwnerId
	LEFT JOIN dbo.People co ON co.PeopleId = t.CoOwnerId
	LEFT JOIN dbo.People ab ON ab.PeopleId = t.WhoId
	WHERE StatusId <> 40
	AND t.CreatedOn >= DATEADD(MONTH, -12, GETDATE())
	AND t.Description <> 'New Person Data Entry'
	AND LEN(t.Description) > 0

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
