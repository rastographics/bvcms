CREATE FUNCTION [dbo].[InvolvementPrevious](@pid INT, @currentUserId int)
RETURNS TABLE
AS
RETURN
(
	WITH UserInfo AS (
		SELECT 
			PeopleId
			, UserId 
			,isaccess = IIF(EXISTS(
				SELECT NULL FROM dbo.UserRole ur 
				JOIN dbo.Roles r ON r.RoleId = ur.RoleId 
				WHERE ur.UserId = u.UserId AND r.RoleName = 'Access'), 1, 0)
			,isorgleader = IIF(EXISTS(
				SELECT NULL FROM dbo.UserRole ur 
				JOIN dbo.Roles r ON r.RoleId = ur.RoleId 
				WHERE ur.UserId = u.userid AND r.RoleName = 'OrgLeadersOnly'), 1, 0)
		FROM dbo.Users u
		WHERE UserId = @currentUserId
	), 
	Settings AS (
		SELECT 
			OrgTypes  = (SELECT Setting FROM dbo.Setting WHERE Id = IIF(
				isaccess = 1 AND isorgleader = 0, 
				'UX-DefaultAccessInvolvementOrgTypeFilter', 
				'UX-DefaultInvolvementOrgTypeFilter'))
		FROM UserInfo
	),
	OrgTypes AS (
		SELECT TokenID, Value = LTRIM(Value)
		FROM Split((SELECT OrgTypes FROM Settings), ',')
	)
	SELECT 
		o.OrganizationId 
		,OrgType = ISNULL(ot.[Description], 'Other')
		,OrgCode = ot.Code
		,OrgTypeSort = ISNULL(ots.TokenID, 9999)
		,[Name] = et.OrganizationName
		,DivisionName = di.Name
		,ProgramName = pr.Name
		,o.LeaderName
		,o.Location
		,MeetingTime = (SELECT TOP 1 os.MeetingTime
					FROM dbo.OrgSchedule os
					WHERE os.OrganizationId = o.OrganizationId)
		,MemberType = mt.[Description]
		,EnrollDate = et.EnrollmentDate
		,TransactionDate = et.TransactionDate
		,FirstTransactionDate = firstEt.TransactionDate
		,et.AttendancePercentage
		,HasDirectory = CAST(IIF(o.PublishDirectory = 1, 1, 0) AS BIT)
		,IsLeaderAttendanceType = CAST(IIF(mt.AttendanceTypeId = 10, 1, 0) AS BIT)
		,et.PeopleId
		,o.LeaderId
		,et.Pending
		,o.LimitToRole
		,o.SecurityTypeId
		,et.TransactionTypeId
		,et.TransactionStatus
	FROM dbo.EnrollmentTransaction et
	JOIN dbo.Organizations o ON o.OrganizationId = et.OrganizationId
	JOIN lookup.MemberType mt ON mt.Id = et.MemberTypeId
	LEFT JOIN dbo.Division di ON di.Id = o.DivisionId
	LEFT JOIN dbo.Program pr ON pr.Id = di.ProgId
	LEFT JOIN dbo.EnrollmentTransaction firstEt ON firstEt.TransactionId = et.EnrollmentTransactionId
	LEFT JOIN lookup.OrganizationType ot ON ot.Id = o.OrganizationTypeId
	LEFT JOIN OrgTypes ots ON ots.Value = ISNULL(ot.[Description], 'Other')
	WHERE et.PeopleId = @pid
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
