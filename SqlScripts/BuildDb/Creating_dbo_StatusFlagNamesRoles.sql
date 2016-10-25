

CREATE VIEW [dbo].[StatusFlagNamesRoles]
AS

select 
	MAX(Id) Id,
	t.Name Flag, 
	REPLACE(REPLACE(SUBSTRING(c.Name, 5, 50), ' ', ''), '.', '_') [Name], 
	r.RoleName [Role]
FROM Tag t
JOIN dbo.Query c ON LEFT(c.Name,3) = t.Name
LEFT JOIN dbo.Roles r ON r.RoleName = 'StatusFlag:' + t.Name
WHERE t.TypeId = 100
GROUP BY t.Name, c.Name, r.RoleName


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
