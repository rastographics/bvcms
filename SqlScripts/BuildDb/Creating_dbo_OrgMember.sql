CREATE FUNCTION [dbo].[OrgMember] 
(
	 @oid INT
	,@grouptype VARCHAR(20)
	,@first VARCHAR(30)
	,@last VARCHAR(30) 
	,@sgfilter VARCHAR(300)
	,@showhidden BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT PeopleId
	FROM (
		SELECT om.PeopleId
		FROM dbo.OrganizationMembers om
		JOIN dbo.People p ON p.PeopleId = om.PeopleId
		WHERE om.OrganizationId = @oid

		AND (@grouptype NOT LIKE '%40%' OR @showhidden = 1 OR ISNULL(om.Hidden,0) = 0)

		AND ((@grouptype LIKE '10%' /* Member */ AND om.MemberTypeId NOT IN (230, 311)) -- not inactive, prospect
		  OR (@grouptype LIKE '%20%' /* Inactive */ AND om.MemberTypeId = 230) -- inactive
		  OR (@grouptype LIKE '%30%' /* Pending */ AND om.Pending = 1)
		  OR (@grouptype LIKE '%40%' /* Prospect */ AND om.MemberTypeId = 311) -- prospect
		)
		AND (ISNULL(LEN(@first), 0) = 0 OR (p.FirstName LIKE (@first + '%') OR p.NickName LIKE (@first + '%')))
		AND (ISNULL(LEN(@last), 0) = 0 OR p.LastName LIKE (@last + '%') OR p.PeopleId = TRY_CONVERT(INT, @last))
		AND 
		( 
			(ISNULL(LEN(@sgfilter), 0) > 0 AND @sgfilter NOT LIKE 'ALL:%' AND @sgfilter <> 'NONE'
				AND EXISTS(SELECT NULL FROM dbo.OrgMemMemTags omt 
					JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId
					WHERE mt.OrgId = om.OrganizationId AND omt.PeopleId = om.PeopleId
				    AND EXISTS(SELECT NULL FROM split(@sgfilter, ',') pf 
						WHERE pf.value NOT LIKE '-%' AND mt.Name LIKE (pf.Value + '%')
					)
				)
			)
			OR (@sgfilter LIKE 'ALL:%' 
				AND (SELECT COUNT(*) FROM split(SUBSTRING(@sgfilter, 5, 200), ',') pf
					 WHERE pf.value NOT LIKE '-%'
				) = (SELECT COUNT(*) FROM dbo.OrgMemMemTags omt 
					 JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId
					 JOIN split(SUBSTRING(@sgfilter, 5, 200), ',') pf ON  mt.Name LIKE (pf.Value + '%')
					 WHERE mt.OrgId = om.OrganizationId AND omt.PeopleId = om.PeopleId
					 AND pf.value NOT LIKE '-%'
				)
			)
			OR (@sgfilter = 'NONE' 
				AND (SELECT COUNT(*) FROM dbo.OrgMemMemTags omt 
					 JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId
					 WHERE mt.OrgId = @oid AND omt.PeopleId = om.PeopleId
				) = 0
			)
			OR @sgfilter IS NULL
			OR LEN(@sgfilter) = 0
		)
		AND (NOT EXISTS(SELECT NULL FROM split(@sgfilter, ',') pf WHERE pf.value LIKE '-%')
			OR (ISNULL(LEN(@sgfilter), 0) > 0 AND @sgfilter NOT LIKE 'ALL:%' AND @sgfilter <> 'NONE' 
				AND NOT EXISTS(SELECT NULL FROM dbo.OrgMemMemTags omt 
					JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId
					WHERE mt.OrgId = @oid AND omt.PeopleId = om.PeopleId
				    AND EXISTS(SELECT NULL FROM split(@sgfilter, ',') pf 
						WHERE pf.value LIKE '-%' AND mt.Name LIKE SUBSTRING(pf.Value,2,200) + '%'
					)
				)
			)
		)

		UNION

		SELECT etd.PeopleId
		FROM dbo.EnrollmentTransaction etd
		JOIN dbo.People p ON p.PeopleId = etd.PeopleId
		WHERE etd.OrganizationId = @oid
		AND @grouptype LIKE '%50%' -- previous
		AND etd.TransactionStatus = 0
		AND etd.TransactionDate = (
			SELECT MAX(TransactionDate)
			FROM dbo.EnrollmentTransaction m
			WHERE m.PeopleId = etd.PeopleId
			AND m.OrganizationId = @oid
			AND m.TransactionTypeId > 3
			AND m.TransactionStatus = 0
		)
		AND CASE WHEN @showhidden = 1
					THEN CAST(CASE WHEN etd.MemberTypeId = 311 THEN 1 ELSE 0 END AS BIT)
					ELSE CAST(CASE WHEN etd.MemberTypeId <> 311 THEN 1 ELSE 0 END AS BIT)
			END = 1
		AND etd.OrganizationId = @oid
		AND etd.TransactionTypeId >= 4 -- drop type
		AND NOT EXISTS( -- not an existing member of org
			SELECT NULL 
			FROM dbo.OrganizationMembers om
			WHERE om.PeopleId = etd.PeopleId 
			AND om.OrganizationId = @oid
		)
		AND (ISNULL(LEN(@first), 0) = 0 OR (p.FirstName LIKE (@first + '%') OR p.NickName LIKE (@first + '%')))
		AND (ISNULL(LEN(@last), 0) = 0 OR p.LastName LIKE (@last + '%') OR p.PeopleId = TRY_CONVERT(INT, @last))
		AND 
		( 
			(ISNULL(LEN(@sgfilter), 0) > 0 AND @sgfilter NOT LIKE 'ALL:%' AND @sgfilter <> 'NONE'
				AND EXISTS(SELECT NULL FROM split(etd.SmallGroups, CHAR(10)) mt
				    WHERE EXISTS(SELECT NULL FROM split(@sgfilter, ',') pf 
						WHERE pf.value NOT LIKE '-%' AND mt.Value LIKE (pf.Value + '%')
					)
				)
			)
			OR (@sgfilter LIKE 'ALL:%' 
				AND (SELECT COUNT(*) FROM split(SUBSTRING(@sgfilter, 5, 200), ',') pf
					 WHERE pf.value NOT LIKE '-%'
				) = (SELECT COUNT(*) FROM split(etd.SmallGroups, CHAR(10)) mt
					 JOIN split(SUBSTRING(@sgfilter, 5, 200), ',') pf ON  mt.Value LIKE (pf.Value + '%')
					 WHERE pf.value NOT LIKE '-%'
				)
			)
			OR (@sgfilter = 'NONE' AND LEN(ISNULL(etd.SmallGroups, '')) = 0)
			OR @sgfilter IS NULL
			OR LEN(@sgfilter) = 0
		)
		AND (NOT EXISTS(SELECT NULL FROM split(@sgfilter, ',') pf WHERE pf.value LIKE '-%')
			OR (ISNULL(LEN(@sgfilter), 0) > 0 AND @sgfilter NOT LIKE 'ALL:%' AND @sgfilter <> 'NONE' 
				AND NOT EXISTS(SELECT NULL FROM split(etd.SmallGroups, CHAR(10)) mt
				    WHERE EXISTS(SELECT NULL FROM split(@sgfilter, ',') pf 
						WHERE pf.value LIKE '-%' AND mt.Value LIKE SUBSTRING(pf.Value,2,200) + '%'
					)
				)
			)
		)

		UNION

		SELECT 
			a.PeopleId
		FROM dbo.Attend a
		JOIN dbo.People p ON p.PeopleId = a.PeopleId
		JOIN dbo.Organizations o ON o.OrganizationId = a.OrganizationId
		LEFT JOIN dbo.OrganizationMembers om ON om.OrganizationId = o.OrganizationId AND om.PeopleId = a.PeopleId
		WHERE a.OrganizationId = @oid
		AND @grouptype LIKE '%60%' -- previous
		AND a.AttendanceFlag = 1
		AND a.AttendanceTypeId IN (40,50,60) -- visited
		AND a.MeetingDate > DATEADD(DAY, -180, GETDATE())
		AND (o.FirstMeetingDate IS NULL OR a.MeetingDate > o.FirstMeetingDate)
		AND a.MeetingDate > (ISNULL(
								(SELECT TOP 1 TransactionDate
								 FROM dbo.EnrollmentTransaction
								 WHERE TransactionTypeId > 3 -- drop type
								 AND PeopleId = a.PeopleId
								 AND OrganizationId = @oid
								 AND MemberTypeId <> 311 -- not dropped as a prospect
								 ORDER BY TransactionDate DESC
							 ), '1/1/1900'))
		AND a.MeetingDate = (SELECT TOP 1 MeetingDate 
							 FROM dbo.Attend
							 WHERE AttendanceFlag = 1 
							 AND OrganizationId = @oid 
							 AND PeopleId = a.PeopleId
							 AND (@showHidden = 1 OR ISNULL(Noshow, 0) = 0)
							 ORDER BY MeetingDate DESC
							)
		AND NOT EXISTS(SELECT NULL FROM OrganizationMembers om
					   WHERE om.OrganizationId = @oid
					   AND om.PeopleId = a.PeopleId
					   AND ISNULL(om.Pending, 0) = 0
					   AND om.MemberTypeId != 311 -- prospect
					   )
		AND (ISNULL(LEN(@first), 0) = 0 OR (p.FirstName LIKE (@first + '%') OR p.NickName LIKE (@first + '%')))
		AND (ISNULL(LEN(@last), 0) = 0 OR p.LastName LIKE (@last + '%') OR p.PeopleId = TRY_CONVERT(INT, @last))
	) tt
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
