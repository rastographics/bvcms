CREATE FUNCTION [dbo].[OrgFilterPeople](
	 @queryid UNIQUEIDENTIFIER
	,@ministryinfo BIT
) RETURNS TABLE
AS
RETURN
(
	SELECT 
		tt.PeopleId
		,Tab
		,GroupCode
		,p.Name
		,p.Name2
		,p.Age
		,p.BirthDay
		,p.BirthMonth
		,p.BirthYear
		,p.IsDeceased
		,p.PrimaryAddress [Address]
		,p.PrimaryAddress2 Address2
		,p.PrimaryCity City
		,p.PrimaryState ST
		,p.PrimaryZip Zip
		,p.EmailAddress
		,p.HomePhone
		,p.CellPhone
		,p.WorkPhone
		,ms.Description MemberStatus
		,bfclass.LeaderId
		,bfclass.LeaderName
		,CAST(CASE WHEN tp.Id IS NULL THEN 0 ELSE 1 END AS BIT) HasTag
		,CAST(CASE WHEN otp.Id IS NULL THEN 0 ELSE 1 END AS BIT) IsChecked
		,AttPct
		,LastAttended
		,Joined
		,Dropped
		,InactiveDate
		,MemberCode
		,MemberType
		,CAST(Hidden AS BIT) Hidden
		,Groups = CHAR(10) + Groups + CHAR(10)
		,mi.LastContactMadeDt
		,mi.LastContactMadeId
		,mi.LastContactReceivedDt
		,mi.LastContactReceivedId
		,mi.TaskAboutDt
		,mi.TaskAboutId
		,mi.TaskDelegatedDt
		,mi.TaskDelegatedId
		
	FROM OrgFilterPeople2(@queryid) tt
	JOIN dbo.OrgFilter f ON f.QueryId = @queryid
	JOIN dbo.People p ON p.PeopleId = tt.PeopleId
	JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
	LEFT JOIN dbo.Organizations bfclass ON bfclass.OrganizationId = p.BibleFellowshipClassId
	LEFT JOIN dbo.TagPerson tp ON tp.Id = f.TagId AND tp.PeopleId = p.PeopleId
	LEFT JOIN Tag ot ON ot.Name = f.QueryId AND ot.TypeId = 3 -- OrgMembersTag
	LEFT JOIN dbo.TagPerson otp ON otp.Id = ot.Id AND otp.PeopleId = p.PeopleId
	LEFT JOIN dbo.MinistryInfo mi ON ISNULL(@ministryinfo, 0) = 1 AND mi.PeopleId = p.PeopleId

	WHERE (ISNULL(LEN(f.FirstName), 0) = 0 OR (p.FirstName LIKE (f.FirstName + '%') OR p.NickName LIKE (f.FirstName + '%')))
	AND (ISNULL(LEN(f.LastName), 0) = 0 OR p.LastName LIKE (f.LastName + '%') OR p.PeopleId = TRY_CONVERT(INT, f.LastName))
	AND (ISNULL(f.FilterIndividuals, 0) = 0 OR otp.Id IS NOT NULL)
	AND (ISNULL(f.FilterTag, 0) = 0 OR tp.Id IS NOT NULL)
	AND 
	( 
		(ISNULL(LEN(f.SgFilter), 0) > 0 AND f.SgFilter NOT LIKE 'ALL:%' AND f.SgFilter <> 'NONE'
			AND EXISTS(SELECT NULL FROM split(tt.Groups, CHAR(10)) mt
			    WHERE EXISTS(SELECT NULL FROM split(f.SgFilter, ';') pf 
					WHERE pf.value NOT LIKE '-%' AND mt.Value LIKE (REPLACE(pf.Value, '*', '%'))
				)
			)
		)
		OR (f.SgFilter LIKE 'ALL:%' 
			AND (SELECT COUNT(*) FROM split(SUBSTRING(f.SgFilter, 5, 200), ';') pf
				 WHERE pf.value NOT LIKE '-%'
			) = (SELECT COUNT(*) FROM split(Groups, CHAR(10)) mt
				 JOIN split(SUBSTRING(f.SgFilter, 5, 200), ';') pf ON  mt.Value LIKE (REPLACE(pf.Value, '*', '%'))
				 WHERE pf.value NOT LIKE '-%'
			)
		)
		OR (f.SgFilter = 'NONE' AND LEN(ISNULL(Groups, '')) = 0)
		OR f.SgFilter IS NULL -- no filter
		OR LEN(f.SgFilter) = 0 -- filter is empty
		-- check to see if they are all exclusion small groups
		OR NOT EXISTS(SELECT NULL FROM split(f.SgFilter, ';') pf WHERE pf.value NOT LIKE '-%')
	)
	AND (NOT EXISTS(SELECT NULL FROM split(f.SgFilter, ';') pf WHERE pf.value LIKE '-%')
		OR (ISNULL(LEN(f.SgFilter), 0) > 0 AND f.SgFilter <> 'NONE' --AND @sgfilter NOT LIKE 'ALL:%' 
			AND NOT EXISTS(SELECT NULL FROM split(Groups, CHAR(10)) mt
			    WHERE EXISTS(SELECT NULL FROM split(f.SgFilter, ';') pf 
					WHERE pf.value LIKE '-%' AND mt.Value LIKE SUBSTRING(REPLACE(pf.Value, '*', '%'),2,200)
				)
			)
		)
	)
	AND (
		 -- Either we are not mixing Previous and Visitors
		 NOT(f.GroupSelect LIKE '%50%' AND f.GroupSelect LIKE '%60%') 
		 -- OR this person is not a previous nor a visitor
		 OR GroupCode NOT IN ('50', '60')
		 -- OR we prefer the Visitor(60) over the Previous(50) if both are in list so we only get one
		 OR GroupCode = (SELECT MAX(GroupCode) 
				FROM 
				(
				SELECT GroupCode FROM dbo.OrgFilterGuests(f.Id, f.ShowHidden) WHERE PeopleId = tt.PeopleId UNION
				SELECT GroupCode FROM dbo.OrgFilterPrevious(f.Id) WHERE PeopleId = tt.PeopleId
				) tabt)
	)
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
