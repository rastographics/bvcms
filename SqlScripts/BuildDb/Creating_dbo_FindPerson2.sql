CREATE FUNCTION [dbo].[FindPerson2](@first nvarchar(25), @goesby nvarchar(50), @last nvarchar(50), @m INT, @d INT, @y INT, @email nvarchar(60), @email2 nvarchar(60), @phone1 nvarchar(15), @phone2 nvarchar(15), @phone3 nvarchar(15))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
	DECLARE @fname nvarchar(50) = REPLACE(@first,' ', '')
	
	SET @phone1 = dbo.SpaceToNull(@phone1)
	SET @phone2 = dbo.SpaceToNull(@phone2)
	SET @phone3 = dbo.SpaceToNull(@phone3)
	SET @email = dbo.SpaceToNull(@email)
	SET @email2 = dbo.SpaceToNull(@email2)
	SET @goesby = dbo.SpaceToNull(@goesby)
	
	INSERT INTO @t SELECT PeopleId FROM dbo.People p
	JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	WHERE
	(
		FirstName2 LIKE (@fname + '%')
		OR @fname LIKE (FirstName + '%')
		OR @fname = NickName
		OR @fname IS NULL
		OR FirstName2 LIKE (@goesby + '%')		
		OR @goesby LIKE (FirstName + '%		')
		OR @goesby = NickName
	)
	AND (@last = LastName OR @last = MaidenName OR @last = MiddleName OR @last IS NULL)
	AND
	(
		p.EmailAddress = @email
		OR p.EmailAddress2 = @email
		OR p.EmailAddress = @email2
		OR p.EmailAddress2 = @email2
		OR f.HomePhone = @phone1
		OR CellPhone = @phone1
		OR WorkPhone = @phone1
		OR f.HomePhone = @phone2		
		OR CellPhone = @phone2
		OR WorkPhone = @phone2
		OR f.HomePhone = @phone3				
		OR CellPhone = @phone3
		OR WorkPhone = @phone3
		OR (BirthDay = @d AND BirthMonth = @m AND BirthYear = @y)
	)
	AND
	(
		BirthDay IS NULL OR BirthMonth IS NULL
		OR (BirthDay = @d AND BirthMonth = @m AND (ABS(BirthYear - @y) <= 1 OR @y IS NULL OR BirthYear IS NULL))
	)
		
	RETURN
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
