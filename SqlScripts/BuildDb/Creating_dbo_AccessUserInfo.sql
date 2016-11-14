CREATE VIEW [dbo].[AccessUserInfo] AS
SELECT 
    u.PeopleId ,
    roles = (SELECT STUFF((
	    SELECT ',' + RoleName
	    FROM dbo.UserRole JOIN dbo.Roles ON Roles.RoleId = UserRole.RoleId WHERE dbo.UserRole.UserId = u.UserId
	    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')),
	lastactive = (SELECT MAX(a.ActivityDate) FROM dbo.ActivityLog a WHERE a.UserId = u.UserId),
	first = p.FirstName,
	goesby = p.NickName,
	last = p.LastName,
	married = p.MaritalStatusId,
	gender = p.GenderId,
	cphone = p.CellPhone,
	hphone = p.HomePhone,
	wphone = p.WorkPhone,
	bday = p.BirthDay,
	bmon = p.BirthMonth,
	byear = p.BirthYear,
	company = p.EmployerOther,
	email = p.EmailAddress,
	emali2 = p.EmailAddress2,
	u.Username
FROM dbo.Users u
JOIN dbo.People p ON p.PeopleId = u.PeopleId
WHERE EXISTS(
	SELECT NULL 
	FROM dbo.UserRole ur 
	JOIN dbo.Roles r ON r.RoleId = ur.RoleId 
	WHERE ur.UserId = u.UserId 
	AND r.RoleName = 'Access')
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
