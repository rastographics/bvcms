CREATE FUNCTION [dbo].[GuestList](
	@oid INT
	,@since DATETIME
	,@showHidden BIT
	,@first VARCHAR(30)
	,@last VARCHAR(30) 
	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		a.PeopleId
		, LastAttendDt = a.MeetingDate
		, ISNULL(a.NoShow, 0) Hidden
		, om.MemberTypeId
	FROM dbo.Attend a
	JOIN dbo.People p ON p.PeopleId = a.PeopleId
	JOIN dbo.Organizations o ON o.OrganizationId = a.OrganizationId
	LEFT JOIN dbo.OrganizationMembers om ON om.OrganizationId = o.OrganizationId AND om.PeopleId = a.PeopleId
	WHERE a.OrganizationId = @oid
	AND a.AttendanceFlag = 1
	AND a.AttendanceTypeId IN (40,50,60) -- visited
	AND a.MeetingDate > @since
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
						 AND (@showHidden = 1 OR ISNULL(Noshow, 0) = 0)
						 ORDER BY MeetingDate DESC
						)
	AND NOT EXISTS(SELECT NULL FROM OrganizationMembers om
				   WHERE om.OrganizationId = @oid
				   AND om.PeopleId = a.PeopleId
				   AND ISNULL(om.Pending, 0) = 0
				   AND om.MemberTypeId != 311 -- prospect
				   )
	AND (ISNULL(LEN(@first), 0) = 0 OR (p.FirstName LIKE (@first + '%') OR p.NickName LIKE (@first + '%')))
	AND (ISNULL(LEN(@last), 0) = 0 OR p.LastName LIKE (@last + '%') OR p.PeopleId = TRY_CONVERT(INT, @last))
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
