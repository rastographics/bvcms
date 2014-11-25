CREATE FUNCTION [dbo].[GuestList](@oid INT, @since DATETIME, @showHidden BIT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT a.PeopleId, a.MeetingId, a.AttendId, a.MeetingDate, ISNULL(a.NoShow, 0) Hidden
	FROM Attend a
	JOIN dbo.Organizations o ON o.OrganizationId = a.OrganizationId
	WHERE a.OrganizationId = @oid
	AND AttendanceFlag = 1
	AND a.AttendanceTypeId IN (40,50,60) -- visited
	AND a.MeetingDate > @since
	AND (o.FirstMeetingDate IS NULL OR a.MeetingDate > o.FirstMeetingDate)
	AND a.MeetingDate > (ISNULL(
							(SELECT TransactionDate FROM dbo.EnrollmentTransaction et
							 WHERE et.TransactionTypeId > 3 -- drop type
							 AND et.PeopleId = a.PeopleId
							 AND et.OrganizationId = @oid
							 AND et.MemberTypeId <> 311 -- not dropped as a prospect
						 ), '1/1/1900'))
	AND a.MeetingDate = (SELECT MAX(la.MeetingDate) FROM dbo.Attend la
						 WHERE la.AttendanceFlag = 1 
						 AND la.OrganizationId = @oid 
						 AND la.PeopleId = a.PeopleId
						 AND (@showHidden = 1 OR ISNULL(la.Noshow, 0) = 0)
						)
	AND NOT EXISTS(SELECT NULL FROM OrganizationMembers om
				   WHERE om.OrganizationId = @oid
				   AND om.PeopleId = a.PeopleId
				   AND ISNULL(om.Pending, 0) = 0
				   AND om.MemberTypeId != 311 -- prospect
				   )
)

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
