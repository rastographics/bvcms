CREATE FUNCTION [dbo].[FindPerson4]
(
	@PeopleId1 INT
)
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
	
	DECLARE @first nvarchar(30), 
			@last nvarchar(50), 
			@m INT, @d INT, @y INT, 
			@email nvarchar(100), 
			@email2 nvarchar(60), 
			@phone1 nvarchar(20),
			@phone2 nvarchar(20),
			@phone3 nvarchar(20),
			@familyid INT
			
	SELECT 
		@first = PreferredName,
		@last = LastName,
		@m = BirthMonth,
		@d = BirthDay,
		@y = BirthYear,
		@email = dbo.SpaceToNull(EmailAddress),
		@email2 = dbo.SpaceToNull(EmailAddress2),
		@phone1 = dbo.SpaceToNull(CellPhone),
		@phone2 = dbo.SpaceToNull(WorkPhone),
		@phone3 = dbo.SpaceToNull(HomePhone),
		@familyid = FamilyId
		
	FROM dbo.People
	WHERE PeopleId = @PeopleId1
	
	DECLARE @fname nvarchar(50) = REPLACE(@first,' ', '')
	
	INSERT INTO @t SELECT PeopleId FROM dbo.People p
	JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	WHERE
	(
		@email IS NOT NULL
		OR @email2 IS NOT NULL
		OR @phone1 IS NOT NULL
		OR @phone2 IS NOT NULL
		OR @phone3 IS NOT NULL
		OR (@m IS NOT NULL AND @d IS NOT NULL AND @y IS NOT NULL)
	)
	AND PeopleId <> @PeopleId1
	AND
	(
		FirstName2 LIKE (@fname + '%')
		OR @fname LIKE (FirstName + '%')
		OR @fname = NickName
		OR @fname IS NULL
	)
	AND (@last = LastName OR @last = MaidenName OR @last = MiddleName)
	AND
	(
		p.EmailAddress = @email
		OR p.EmailAddress = @email2
		OR p.EmailAddress2 = @email
		OR p.EmailAddress2 = @email2
		OR CellPhone = @phone1
		OR WorkPhone = @phone1
		OR CellPhone = @phone2
		OR WorkPhone = @phone2
		OR CellPhone = @phone3
		OR WorkPhone = @phone3
		OR (BirthDay = @d AND BirthMonth = @m AND BirthYear = @y)
		OR 
		( p.FamilyId <> @familyid
		  AND
		  ( f.HomePhone = @phone1		
			OR f.HomePhone = @phone2				
			OR f.HomePhone = @phone3				
		  )
		)
	)
	AND
	(
		@d IS NULL OR BirthDay IS NULL
		OR (BirthDay = @d AND BirthMonth = @m AND ABS(BirthYear - @y) <= 1)
	)
		
    RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
