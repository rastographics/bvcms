


CREATE VIEW [dbo].[MinistryInfo]
AS
SELECT tt.PeopleId
	, LastContactReceivedId = ce.ContactId
	, LastContactReceivedDt = ce.ContactDate 
	, LastContactMadeId = co.ContactId
	, LastContactMadeDt = co.ContactDate 
	, LastTaskAboutId = tab.Id
	, LastTaskAboutDt = tab.CreatedOn
	, LastTaskDelegatedId = tmi.Id
	, LastTaskDelegatedDt = tmi.CreatedOn
FROM
(
	SELECT
	     p.PeopleId
		 , ContacteeId = (SELECT TOP 1 ce.ContactId
	             FROM dbo.Contactees ce
				 JOIN dbo.Contact ON Contact.ContactId = ce.ContactId
	             WHERE PeopleId = p.PeopleId
	             ORDER BY ContactDate DESC, ce.ContactId DESC
	            )
		 , ContactorId = (SELECT TOP 1 co.ContactId
	             FROM dbo.Contactees co
				 JOIN dbo.Contact ON Contact.ContactId = co.ContactId
	             WHERE PeopleId = p.PeopleId
	             ORDER BY ContactDate DESC, co.ContactId DESC
	            )
		, TaskAboutId = (SELECT TOP 1 t.Id
				 FROM dbo.Task t
				 WHERE t.WhoId = p.PeopleId
				 ORDER BY t.CreatedOn DESC
				)
		, TaskDelegateId = (SELECT TOP 1 t.Id
				 FROM dbo.Task t
				 WHERE t.CoOwnerId = p.PeopleId
				 ORDER BY t.CreatedOn DESC
				)
	FROM dbo.People p
) tt 
LEFT JOIN dbo.Contact ce ON ce.ContactId = tt.ContacteeId
LEFT JOIN dbo.Contact co ON co.ContactId = tt.ContactorId
LEFT JOIN dbo.Task TAB ON TAB.Id = tt.TaskAboutId
LEFT JOIN dbo.Task tmi ON tmi.Id = tt.TaskDelegateId


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
