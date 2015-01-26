CREATE VIEW [dbo].[FailedEmails]
AS
SELECT 
	f.time,
	et.Id, 
	et.PeopleId, 
	CASE WHEN f.[event] = 'spamreport' THEN 'spamreport'
		 WHEN f.[event] = 'bounce' THEN f.bouncetype
		 WHEN f.[event] = 'dropped' THEN 
			CASE WHEN f.reason = 'Spam Reporting Address' THEN 'spamreporting'
				 WHEN f.reason = 'Spam Content' THEN 'spamcontent'
				 WHEN f.reason = 'Bounced Address' THEN 'bouncedaddress'
				 WHEN f.reason = 'Invalid' THEN 'invalid'
			END
	END Fail

FROM dbo.EmailQueueToFail f
JOIN dbo.EmailQueueTo et ON et.Id = f.Id AND et.PeopleId = f.PeopleId
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
