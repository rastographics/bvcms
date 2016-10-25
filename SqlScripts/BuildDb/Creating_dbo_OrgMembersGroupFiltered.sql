CREATE FUNCTION [dbo].[OrgMembersGroupFiltered](@oids VARCHAR(MAX), @sgfilter VARCHAR(200))
RETURNS 
@t TABLE 
(
	PeopleId INT,
	OrganizationId INT,
	Age INT,
	Grade INT,
	MemberTypeId INT,
	MemberType VARCHAR(100),
	BirthYear INT,
	BirthMonth INT,
	BirthDay INT,
	OrganizationName NVARCHAR(100),
	Name2 NVARCHAR(200),
	Name NVARCHAR(200),
	Gender VARCHAR(10),
	HashNum INT,
	Request NVARCHAR(140),
	Groups NVARCHAR(MAX)
)
AS
BEGIN
	DECLARE @oid INT

	DECLARE c CURSOR FOR
	SELECT Value
	FROM dbo.SplitInts(@oids)

	OPEN c
	FETCH NEXT FROM c INTO @oid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT @t
		        ( PeopleId
				,OrganizationId
		        ,Age
		        ,Grade
		        ,MemberTypeId
				,MemberType
		        ,BirthYear
		        ,BirthMonth
		        ,BirthDay
		        ,OrganizationName
		        ,Name2
		        ,Name
		        ,Gender
		        ,HashNum
				,Request
		        ,Groups
		        )
		SELECT 
			op.PeopleId, 
			om.OrganizationId,
			p.Age, 
			p.Grade, 
			om.MemberTypeId, 
			mt.[Description],
			p.BirthYear,
			p.BirthMonth,
			p.BirthDay,
			o.OrganizationName,
			p.Name2,
			p.Name,
			g.Code,
			p.HashNum,
			om.Request,
			op.Groups
		FROM dbo.OrgPeople(@oid, '10', NULL, NULL, @sgfilter, 0, NULL, NULL, 0, 0, 0, NULL) op
		JOIN dbo.People p ON p.PeopleId = op.PeopleId
		JOIN lookup.Gender g ON g.Id = p.GenderId
		JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId AND om.OrganizationId = @oid
		JOIN lookup.MemberType mt ON mt.Id = om.MemberTypeId
		JOIN dbo.Organizations o ON o.OrganizationId = om.OrganizationId
		LEFT JOIN @t pt ON pt.PeopleId = p.PeopleId
		WHERE pt.PeopleId IS NULL

		FETCH NEXT FROM c INTO @oid
	END
	CLOSE c
	DEALLOCATE c

	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
