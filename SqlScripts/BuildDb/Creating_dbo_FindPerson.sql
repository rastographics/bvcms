CREATE FUNCTION [dbo].[FindPerson](@first nvarchar(25), @last nvarchar(50), @dob DATETIME, @email nvarchar(60), @phone nvarchar(15))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
--DECLARE @t TABLE ( PeopleId INT)
--DECLARE @first nvarchar(25) = 'Beth', 
--		@last nvarchar(50) = 'Marcus', 
--		@dob DATETIME = '1/29/2013', 
--		@email nvarchar(60) = 'b@b.com', 
--		@phone nvarchar(15) = '9017581862'

--SELECT CONVERT(VARCHAR, N'àéêöhello!') COLLATE SQL_Latin1_General_CP1253_CI_AI
		
	DECLARE @fname nvarchar(50) = (SELECT CONVERT(VARCHAR, REPLACE(@first,' ', '')) COLLATE SQL_Latin1_General_CP1253_CI_AI)
	DECLARE @lname nvarchar(50) = (SELECT CONVERT(VARCHAR, REPLACE(@last,' ', '')) COLLATE SQL_Latin1_General_CP1253_CI_AI)

	SET @dob = CASE WHEN @dob = '' THEN NULL ELSE @dob END
	
	DECLARE @m INT = DATEPART(m, @dob)
	DECLARE @d INT = DATEPART(d, @dob)
	DECLARE @y INT = DATEPART(yy, @dob)
	
	DECLARE @mm TABLE ( PeopleId INT, Matches INT )
	SET @phone = dbo.GetDigits(@phone)
	
	INSERT INTO @mm
	SELECT PeopleId, -- col 1
	(
		CASE WHEN (ISNULL(@email, '') = '' AND ISNULL(@phone, '') = '' AND @dob IS NULL)
		OR (p.EmailAddress = @email AND LEN(@email) > 0)
		OR (p.EmailAddress2 = @email AND LEN(@email) > 0) 
		OR EXISTS(SELECT NULL FROM dbo.People m 
			WHERE m.FamilyId = p.FamilyId AND m.PeopleId <> p.PeopleId 
			AND m.EmailAddress = @email AND LEN(@email) > 0) 
		THEN 1 ELSE 0 END 
		+
		CASE WHEN (f.HomePhone = @phone AND LEN(@phone) > 0)
		OR (p.CellPhone = @phone AND LEN(@phone) > 0)
		OR EXISTS(SELECT NULL FROM dbo.People m 
			WHERE m.FamilyId = p.FamilyId AND m.PeopleId <> p.PeopleId 
			AND m.CellPhone = @phone AND LEN(@phone) > 0) 
		THEN 1 ELSE 0 END 
		+
		CASE WHEN (BirthDay = @d AND BirthMonth = @m AND ABS(BirthYear - @y) <= 1)
		OR (BirthDay = @d AND BirthMonth = @m AND BirthYear IS NULL)
		THEN 1 ELSE 0 END
	) matches -- col 2
	FROM dbo.SearchNoDiacritics p
	JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	WHERE
	(
		   @fname = REPLACE(FirstName, ' ', '')
		OR @fname = REPLACE(NickName, ' ', '')
		OR FirstName2 LIKE (@fname + '%')
		OR @fname LIKE (FirstName + '%')
	)
	AND (@lname = REPLACE(LastName, ' ', '')
		OR @lname = MaidenName 
		OR @lname = MiddleName)
	
	
	INSERT INTO @t
	SELECT PeopleId FROM @mm m1
	WHERE m1.Matches = (SELECT MAX(Matches) FROM @mm) AND m1.Matches > 0
	ORDER BY Matches DESC
	
	--SELECT * FROM @t
	
	RETURN
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
