CREATE VIEW [dbo].[StatusFlagList]
	AS 
		SELECT sf.Flag, sf.Name, r.RoleName, 
		CAST (CASE WHEN ff.Value IS NOT NULL THEN 1 ELSE 0 END AS BIT) Visible
	FROM
	(
		SELECT SUBSTRING(c.Name, 1,3) Flag, SUBSTRING(c.Name, 5, 100) Name
		FROM dbo.Query c 
		WHERE c.Name LIKE 'F[0-9][0-9]:%'
	) sf
	LEFT JOIN dbo.Split((SELECT Setting FROM Setting WHERE Id = 'StatusFlags'), ',') ff ON sf.Flag = ff.Value
	LEFT JOIN dbo.Roles r ON r.RoleName = 'StatusFlag:' + Flag
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
