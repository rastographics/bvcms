CREATE PROCEDURE [dbo].[HomePage] ( @PeopleId INT)
AS
BEGIN

	IF EXISTS(SELECT NULL FROM Tag WHERE PeopleId = @PeopleId)
		SELECT dbo.NextBirthday(p.PeopleId) Birthday, p.Name, p.PeopleId  
		FROM dbo.Tag t
		JOIN dbo.People op ON t.PeopleId = op.PeopleId
		JOIN dbo.TagPerson tp ON t.Id = tp.Id
		JOIN dbo.People p ON tp.PeopleId = p.PeopleId
		WHERE t.Name = 'TrackBirthdays'
		AND t.PeopleId = @PeopleId
		AND DATEDIFF(d, GETDATE(), dbo.NextBirthday(p.PeopleId)) <= 15
		AND op.DeceasedDate IS NULL
		ORDER BY dbo.NextBirthday(p.PeopleId)
	ELSE
		SELECT dbo.NextBirthday(p.PeopleId) Birthday, p.Name, p.PeopleId
		FROM dbo.OrganizationMembers om 
		JOIN dbo.People p ON om.PeopleId = p.PeopleId
		WHERE om.OrganizationId = p.BibleFellowshipClassId
		
	DECLARE @limitvisibility BIT = 0
	IF EXISTS(
		SELECT NULL 
		FROM dbo.UserRole ur 
		JOIN dbo.Roles r ON ur.RoleId = r.RoleId 
		JOIN dbo.Users u ON ur.UserId = u.UserId
		WHERE r.RoleName = 'OrgLeadersOnly' 
		AND u.PeopleId = @PeopleId)
		SET @limitvisibility = 1
	
	DECLARE @oids TABLE ( OrganizationId int )

	IF @limitvisibility = 1
	BEGIN
		INSERT INTO @oids (OrganizationId)
			SELECT o.OrganizationId
			FROM dbo.Organizations o
			WHERE EXISTS(
				SELECT NULL 
				FROM dbo.OrganizationMembers om
				JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
				WHERE om.OrganizationId = o.OrganizationId
				AND om.PeopleId = @PeopleId
				AND mt.AttendanceTypeId =  10)
		INSERT INTO @oids ( OrganizationId )
			SELECT o.OrganizationId
			FROM dbo.Organizations o
			JOIN dbo.Organizations co ON o.OrganizationId = co.ParentOrgId
			WHERE o.OrganizationId IN (SELECT OrganizationId FROM @oids)
		INSERT INTO @oids ( OrganizationId )
			SELECT o.OrganizationId
			FROM dbo.Organizations o
			JOIN dbo.Organizations co ON o.OrganizationId = co.ParentOrgId
			JOIN dbo.Organizations cco ON co.OrganizationId = cco.ParentOrgId
			WHERE o.OrganizationId IN (SELECT OrganizationId FROM @oids)
	END
	
	SELECT o.OrganizationName AS Name, mt.Description MemberType, o.OrganizationId OrgId
	FROM dbo.OrganizationMembers om
	JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
	JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
	JOIN dbo.People p ON om.PeopleId = p.PeopleId
	WHERE om.PeopleId = @PeopleId
	AND ISNULL(om.Pending, 0) = 0
	AND (om.OrganizationId IN (
			SELECT OrganizationId FROM @oids) 
			OR NOT(@limitvisibility = 1 AND o.SecurityTypeId = 3))
	ORDER BY o.OrganizationName
	

	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
