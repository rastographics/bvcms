CREATE FUNCTION [dbo].[FilterOrgSearchName](@name VARCHAR(100))
RETURNS 
@t TABLE ( oid INT )
AS
BEGIN

	DECLARE @oids TABLE (oid INT)
	INSERT @oids ( oid ) SELECT Value FROM dbo.SplitInts(@name)
	IF LEN(ISNULL(@name,'')) > 0 AND EXISTS(SELECT NULL FROM @oids) AND NOT EXISTS(SELECT NULL FROM @oids WHERE oid IS NULL)
	BEGIN
		INSERT @t SELECT oid FROM @oids oo
		JOIN dbo.Organizations o ON o.OrganizationId = oo.oid
		RETURN
	END
        
	;WITH filterNames AS (
		SELECT OrganizationId oid
		FROM dbo.Organizations o
		WHERE ( 
			o.OrganizationName LIKE '%' + @name + '%' 
			OR o.Location LIKE '%' + @name + '%'
			OR o.PendingLoc LIKE '%' + @name + '%'
			OR o.LeaderName LIKE '%' + @name + '%'
			OR EXISTS(
				SELECT NULL
				FROM dbo.DivOrg dd
				JOIN dbo.Division d ON d.Id = dd.DivId
				WHERE d.Name LIKE '%' + @name + '%'
				AND dd.OrgId = o.OrganizationId
			)
		)
	),
	filterHasExtraValue AS (
		SELECT OrganizationId oid
		FROM dbo.Organizations o
		WHERE EXISTS(
			SELECT NULL
			FROM OrganizationExtra e
			WHERE e.OrganizationId = o.OrganizationId
			AND e.Field LIKE SUBSTRING(@name, 4, 50) + '%'
		)
	),
	filterNotHasExtraValue AS (
		SELECT OrganizationId oid
		FROM dbo.Organizations o
		WHERE NOT EXISTS(
			SELECT NULL
			FROM OrganizationExtra e
			WHERE e.OrganizationId = o.OrganizationId
			AND e.Field LIKE SUBSTRING(@name, 5, 50) + '%'
		)
	),
	token2 AS (
		SELECT CAST(Value AS INT) oid FROM dbo.Split(@name, ':') WHERE TokenID = 2
	),
	filterPicklist AS (
		SELECT oid FROM token2
		UNION
		SELECT PickListOrgId
		FROM dbo.MasterOrgs 
		WHERE OrganizationId = (SELECT oid FROM token2)
	),
	filterChildOf AS (
		SELECT oid FROM token2
		UNION
		SELECT OrganizationId
		FROM dbo.Organizations
		WHERE ParentOrgId = (SELECT oid FROM token2)
	),
	filterRegSettings AS (
		SELECT OrganizationId oid
		FROM dbo.RegsettingUsage
		WHERE ( SELECT COUNT(*)
				FROM dbo.Split(SUBSTRING(@name,12,1000), ',') n
				JOIN dbo.Split(Usage, ' ') u ON u.Value = n.Value ) 
			= ( SELECT COUNT(*) FROM dbo.Split(SUBSTRING(@name,12,1000), ',') )
	),
	filterName AS (
		SELECT OrganizationId oid
		FROM dbo.Organizations o
		WHERE (@name LIKE 'ev:%' AND o.OrganizationId IN (SELECT oid FROM filterHasExtraValue)) 
		OR (@name LIKE '-ev:%' AND o.OrganizationId IN (SELECT oid FROM filterNotHasExtraValue)) 
		OR (@name LIKE 'childof:%' AND o.OrganizationId IN (SELECT oid FROM filterChildOf)) 
		OR (@name LIKE 'master:%' AND o.OrganizationId IN (SELECT oid FROM filterPicklist)) 
		OR (@name LIKE 'regsetting:%' AND o.OrganizationId IN (SELECT oid FROM filterRegSettings)) 
		OR (o.OrganizationId IN (SELECT oid FROM filterNames)) 
	)
	INSERT @t SELECT oid FROM filterName
	RETURN 
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
