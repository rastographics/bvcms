
CREATE FUNCTION [dbo].[OrgFilterPrevious](@oid INT)
RETURNS TABLE
AS
RETURN
(
	SELECT etd.PeopleId
	, 'Previous' Tab
	, '50' GroupCode
	, etd.AttendancePercentage AttPct
	, a.MeetingDate LastAttended
	, etj.EnrollmentDate Joined
	, etd.TransactionDate Dropped
	, etd.InactiveDate
	, mt.Code MemberCode
	, mt.Description MemberType
	, 0 Hidden
	, etd.SmallGroups Groups

	FROM dbo.EnrollmentTransaction etd
	LEFT JOIN dbo.Attend a ON a.OrganizationId = etd.OrganizationId 
			AND a.PeopleId = etd.PeopleId AND a.AttendanceFlag = 1
			AND a.MeetingDate = (SELECT MAX(la.MeetingDate)
					FROM dbo.Attend la
					WHERE la.OrganizationId = @oid
					AND la.PeopleId = a.PeopleId
					AND la.AttendanceFlag = 1)
	LEFT JOIN dbo.EnrollmentTransaction etj ON etj.TransactionId = etd.EnrollmentTransactionId
	LEFT JOIN lookup.MemberType mt ON mt.Id = etd.MemberTypeId
	WHERE etd.OrganizationId = @oid
	AND etd.TransactionStatus = 0
	AND etd.TransactionDate = (
		SELECT MAX(TransactionDate)
		FROM dbo.EnrollmentTransaction m
		WHERE m.PeopleId = etd.PeopleId
		AND m.OrganizationId = @oid
		AND m.TransactionTypeId > 3
		AND m.TransactionStatus = 0
	)
	AND etd.TransactionTypeId >= 4 -- drop type
	AND NOT EXISTS( -- not an existing member of org
		SELECT NULL 
		FROM dbo.OrganizationMembers om
		WHERE om.PeopleId = etd.PeopleId 
		AND om.OrganizationId = @oid
	)
)


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
