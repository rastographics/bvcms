CREATE FUNCTION [dbo].[FindPerson3](@first nvarchar(25), @last nvarchar(50), @dob DATETIME, @email nvarchar(60), 
			@phone1 nvarchar(15),
			@phone2 nvarchar(15),
			@phone3 nvarchar(15))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
	DECLARE @mm TABLE ( PeopleId INT, Matches INT )
	
	DECLARE @fname nvarchar(50) = REPLACE(@first,' ', '')
	SET @dob = CASE WHEN @dob = '' THEN NULL ELSE @dob END
	
	DECLARE @m INT = DATEPART(m, @dob)
	DECLARE @d INT = DATEPART(d, @dob)
	DECLARE @y INT = DATEPART(yy, @dob)
	
	SET @phone1 = dbo.SpaceToNull(dbo.GetDigits(@phone1))
	SET @phone2 = dbo.SpaceToNull(dbo.GetDigits(@phone2))
	SET @phone3 = dbo.SpaceToNull(dbo.GetDigits(@phone3))
	SET @email = dbo.SpaceToNull(@email)
	
	INSERT INTO @mm
	SELECT PeopleId, -- col 1
	(
		CASE WHEN (ISNULL(@email, '') = '' 
					AND ISNULL(@phone1, '') = '' 
					AND ISNULL(@phone2, '') = '' 
					AND ISNULL(@phone3, '') = '' 
					AND @dob IS NULL)
		OR (p.EmailAddress = @email AND LEN(@email) > 0)
		OR (p.EmailAddress2 = @email AND LEN(@email) > 0) 
		THEN 1 ELSE 0 END 
		+
		CASE WHEN (
					f.HomePhone = @phone1 AND LEN(@phone1) > 0)
				OR (CellPhone = @phone1 AND LEN(@phone1) > 0)
				OR (WorkPhone = @phone1 AND LEN(@phone1) > 0)
				OR (f.HomePhone = @phone2 AND LEN(@phone2) > 0)
				OR (CellPhone = @phone2 AND LEN(@phone2) > 0)
				OR (WorkPhone = @phone2 AND LEN(@phone2) > 0)
				OR (f.HomePhone = @phone3 AND LEN(@phone3) > 0)
				OR (CellPhone = @phone3 AND LEN(@phone3) > 0)
				OR (WorkPhone = @phone3 AND LEN(@phone3) > 0)
		THEN 1 ELSE 0 END 
		+
		CASE WHEN (BirthDay = @d AND BirthMonth = @m AND ABS(BirthYear - @y) <= 1)
		OR (BirthDay = @d AND BirthMonth = @m AND BirthYear IS NULL)
		THEN 1 ELSE 0 END
	) matches -- col 2
	FROM dbo.People p
	JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	WHERE
	(
		   @fname = FirstName
		OR @fname = NickName
		OR FirstName2 LIKE (@fname + '%')
		OR @fname LIKE (FirstName + '%')
	)
	AND (@last = LastName OR @last = MaidenName OR @last = MiddleName)
	
	
	INSERT INTO @t
	SELECT PeopleId FROM @mm m1
	WHERE m1.Matches = (SELECT MAX(Matches) FROM @mm) AND m1.Matches > 0
	ORDER BY Matches DESC
		
    RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
