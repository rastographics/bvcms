CREATE PROCEDURE [dbo].[CleanStart]
AS

DELETE dbo.ActivityLog;
DELETE RssFeed;
DELETE ChangeLog;
DELETE DeleteMeetingRun
DELETE EmailToText;
DELETE Numbers;
DELETE SecurityCodes;
DELETE TaskList
DELETE TransactionPeople
DELETE [Transaction];
DELETE Tag;
DELETE Preferences;
DELETE FamilyCheckinLock;
DELETE EmailQueueTo;
DELETE EmailQueue;
DELETE VolInterestInterestCodes;
DELETE VolInterestCodes;
DELETE dbo.LongRunningOperation

DELETE TagPerson 
FROM dbo.TagPerson tp 
JOIN tag t ON tp.Id = t.Id 
WHERE t.TypeId = 1

UPDATE Users 
SET Password = '2352354235', 
	TempPassword = 'bvcms', 
	LastLoginDate = NULL 
WHERE username = 'admin'

DELETE dbo.Setting 
WHERE id IN ('LastBatchRun', 'LastOrgFilterCleanup')

UPDATE dbo.Content
SET ThumbID = 0

RETURN 0
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
