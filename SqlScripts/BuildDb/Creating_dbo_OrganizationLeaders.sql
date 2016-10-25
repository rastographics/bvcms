CREATE VIEW [dbo].[OrganizationLeaders]
AS
SELECT DISTINCT * FROM
(

SELECT om.PeopleId, om.OrganizationId 
FROM dbo.OrganizationMembers om
JOIN lookup.MemberType t ON om.MemberTypeId = t.Id
WHERE t.AttendanceTypeId = 10
GROUP BY om.PeopleId, om.OrganizationId

UNION

SELECT om.PeopleId, o.OrganizationId 
FROM dbo.Organizations po
JOIN dbo.OrganizationMembers om ON po.OrganizationId = om.OrganizationId 
JOIN dbo.Organizations o ON o.ParentOrgId = po.OrganizationId
JOIN lookup.MemberType t ON om.MemberTypeId = t.Id
WHERE t.AttendanceTypeId = 10
GROUP BY om.PeopleId, o.OrganizationId

) tt

WHERE EXISTS(
		SELECT NULL FROM Users u
		JOIN dbo.UserRole ur ON u.UserId = ur.UserId
		JOIN dbo.Roles r ON ur.RoleId = r.RoleId
		WHERE r.RoleName = 'Access'
		AND u.PeopleId = tt.PeopleId )

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
