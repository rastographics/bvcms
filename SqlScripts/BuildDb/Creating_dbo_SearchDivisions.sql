CREATE FUNCTION [dbo].[SearchDivisions] ( @oid INT, @name VARCHAR(100) )
RETURNS TABLE 
AS
RETURN 
(
	SELECT DivId, Division, Program, Programs, IsChecked, IsMain
	FROM 
	(
		SELECT 
			d.Id DivId
			,d.Name Division
			,(SELECT p.Name 
				FROM dbo.Program p 
				WHERE d.ProgId = p.Id 
			) Program
			,CAST(CASE WHEN EXISTS(
						SELECT NULL
						FROM DivOrg dd
						WHERE dd.OrgId = @oid
						AND dd.DivId = d.Id
					) THEN 1 ELSE 0 END AS BIT
			) IsChecked
			,CAST(CASE WHEN EXISTS(
						SELECT NULL
						FROM dbo.Organizations o
						WHERE o.OrganizationId = @oid
						AND o.DivisionId = d.Id
					) THEN 1 ELSE 0 END AS BIT
			) IsMain
			,SUBSTRING(
		        (
		            SELECT  '|'+pp.Name  AS [text()]
		            FROM dbo.ProgDiv pd
					JOIN dbo.Program pp ON pp.Id = pd.ProgId
		            WHERE pd.DivId = d.Id
					AND pp.Id <> d.ProgId -- not the main program
		            ORDER BY pp.Id
		            FOR XML PATH ('')
		        ), 2, 1000
			) Programs -- Other Programs
		FROM dbo.Division d
	) tt
	WHERE IsChecked=1 
		OR @name IS NULL 
		OR LEN(@name) = 0 
		OR Division LIKE '%' + @name + '%' 
		OR Program LIKE '%' + @name + '%'
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
