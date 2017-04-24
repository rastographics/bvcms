
 
CREATE FUNCTION [dbo].[OrgFilterGuests](@oid INT, @showhidden BIT) 
RETURNS TABLE 
AS 
RETURN 
( 
	SELECT a.PeopleId 
	, 'Guests' Tab 
	, '60' GroupCode 
	, NULL AttPct 
	, a.MeetingDate LastAttended 
	, CAST(NULL AS DATETIME) AS Joined 
	, CAST(NULL AS DATETIME) AS Dropped 
	, CAST(NULL AS DATETIME) InactiveDate 
	, at.Code MemberCode 
	, at.Description MemberType 
	, ISNULL(a.NoShow, 0) Hidden 
	, CAST(NULL AS NVARCHAR(MAX)) Groups 
 
	FROM dbo.Attend a 
	JOIN dbo.People p ON p.PeopleId = a.PeopleId 
	JOIN dbo.Organizations o ON o.OrganizationId = a.OrganizationId 
	JOIN lookup.AttendType at ON at.Id = a.AttendanceTypeId 
	WHERE a.OrganizationId = @oid 
	AND a.AttendanceFlag = 1 
	AND a.AttendanceTypeId IN (40,50,60) -- visited 
	AND a.MeetingDate > DATEADD(DAY, -ISNULL((SELECT CONVERT(INT, Setting) FROM dbo.Setting WHERE Id = 'ResetNewVisitorDays'), 180), GETDATE()) 
	AND (o.FirstMeetingDate IS NULL OR a.MeetingDate > o.FirstMeetingDate) 
	AND a.MeetingDate > (ISNULL( 
							(SELECT TOP 1 TransactionDate 
								FROM dbo.EnrollmentTransaction 
								WHERE TransactionTypeId > 3 -- drop type 
								AND PeopleId = a.PeopleId 
								AND OrganizationId = @oid 
								AND MemberTypeId <> 311 -- not dropped as a prospect 
								ORDER BY TransactionDate DESC 
							), '1/1/1900')) 
	AND a.MeetingDate = (SELECT TOP 1 MeetingDate  
							FROM dbo.Attend 
							WHERE AttendanceFlag = 1  
							AND OrganizationId = @oid  
							AND PeopleId = a.PeopleId 
							ORDER BY MeetingDate DESC 
						) 
	AND NOT EXISTS( -- not an existing member of org (in another tab) 
		SELECT NULL  
		FROM dbo.OrganizationMembers om 
		WHERE om.PeopleId = a.PeopleId  
		AND om.OrganizationId = @oid 
		AND om.MemberTypeId <> 311 
	) 
	AND (@showhidden = 1 OR ISNULL(a.NoShow, 0) = 0) 
) 

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
