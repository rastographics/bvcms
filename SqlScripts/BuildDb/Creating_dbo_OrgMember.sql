CREATE FUNCTION [dbo].[OrgMember] 
(
	 @oid INT
	,@grouptype INT
	,@first VARCHAR(30)
	,@last VARCHAR(30) 
	,@sgprefix VARCHAR(30)
	,@groups VARCHAR(1000)
	,@groupsmode INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT om.PeopleId
	FROM dbo.OrganizationMembers om
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	WHERE om.OrganizationId = @oid

	AND ((@grouptype = 10 /* Member */ AND om.MemberTypeId NOT IN (230, 311)) -- not inactive, prospect
	  OR (@grouptype = 20 /* Inactive */ AND om.MemberTypeId = 230) -- inactive
	  OR (@grouptype = 30 /* Pending */ AND om.Pending = 1)
	  OR (@grouptype = 40 /* Prospect */ AND om.MemberTypeId = 311) -- prospect
	)
	AND (ISNULL(LEN(@first), 0) = 0 OR (p.FirstName LIKE @first + '%' OR p.NickName LIKE @first + '%'))
	AND (ISNULL(LEN(@last), 0) = 0 OR p.LastName LIKE @last + '%' OR p.PeopleId = CONVERT(INT, @last))
	AND (ISNULL(LEN(@sgprefix), 0) = 0 OR NOT EXISTS(SELECT NULL 
		FROM dbo.OrgMemMemTags omt 
		JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId
		WHERE EXISTS(SELECT NULL FROM split(@sgprefix, ',') pf WHERE pf.Value LIKE '-%' AND mt.Name LIKE SUBSTRING(pf.Value, 2, 50) + '%'))
	)
	AND (ISNULL(LEN(@sgprefix), 0) = 0 OR EXISTS(SELECT NULL 
		FROM dbo.OrgMemMemTags omt 
		JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId
		WHERE EXISTS(SELECT NULL FROM split(@sgprefix, ',') pf WHERE pf.value NOT LIKE '-%' AND mt.Name LIKE pf.Value + '%'))
	)

	-- Match Any of @groups
	AND (ISNULL(@groupsmode, 0) = 1 
		OR (SELECT COUNT(*) FROM dbo.OrgMemMemTags 
			WHERE OrgId = @oid
			AND PeopleId = om.PeopleId
			AND MemberTagId IN (SELECT Value FROM split(@groups, ','))) > 0 
		OR (SELECT TOP 1 Value FROM SplitInts(@groups)) <= 0
	)

	-- Match All of @groups
	AND (ISNULL(@groupsmode, 0) = 0 
		OR (SELECT COUNT(*) FROM dbo.OrgMemMemTags 
			WHERE OrgId = @oid
			AND PeopleId = om.PeopleId
			AND MemberTagId IN (SELECT Value FROM split(@groups, ',')))
		  =(SELECT COUNT(*) Value FROM dbo.SplitInts(@groups))
		OR (SELECT TOP 1 Value FROM SplitInts(@groups)) <= 0
	)

	-- Match those with no small group assigned
	AND ((SELECT TOP 1 Value FROM SplitInts(@groups)) <> -1
		OR (SELECT COUNT(*) FROM dbo.OrgMemMemTags 
			WHERE OrgId = @oid
			AND PeopleId = om.PeopleId
			AND MemberTagId IN (SELECT Value FROM split(@groups, ','))) = 0 
	)
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
