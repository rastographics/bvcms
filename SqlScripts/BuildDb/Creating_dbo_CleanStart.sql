CREATE PROCEDURE [dbo].[CleanStart]
AS

delete dbo.ActivityLog
delete RssFeed
delete ChangeLog
delete DeleteMeetingRun
delete EmailToText
delete Numbers
delete SecurityCodes
delete TaskList
delete TransactionPeople
delete [Transaction]
delete Tag
delete Preferences
delete FamilyCheckinLock
delete EmailQueueTo
delete EmailQueue
delete VolInterestInterestCodes
delete VolInterestCodes

DELETE TagPerson 
FROM dbo.TagPerson tp 
JOIN tag t ON tp.Id = t.Id 
WHERE t.TypeId = 1

UPDATE Users 
SET Password = '2352354235', 
	TempPassword = 'bvcms', 
	LastLoginDate = NULL 
where username = 'admin'

delete dbo.Setting 
where id = 'LastBatchRun'

RETURN 0
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
