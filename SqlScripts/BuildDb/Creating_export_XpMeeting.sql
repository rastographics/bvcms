CREATE VIEW [export].[XpMeeting] AS 
SELECT 
	MeetingId ,
    OrganizationId ,
    GroupMeetingFlag ,
    [Description] ,
    [Location] ,
    NumPresent ,
    NumMembers ,
    NumVstMembers ,
    NumRepeatVst ,
    NumNewVisit ,
    MeetingDate ,
    NumOutTown ,
    NumOtherAttends ,
    HeadCount 
FROM dbo.Meetings
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
