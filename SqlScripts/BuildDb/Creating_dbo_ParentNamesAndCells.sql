CREATE FUNCTION [dbo].[ParentNamesAndCells](@pid INT) 

RETURNS nvarchar(200)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @name1 nvarchar(30)
	DECLARE @cell1 nvarchar(20)
	DECLARE @name2 nvarchar(30)
	DECLARE @cell2 nvarchar(20)
	DECLARE @names nvarchar(100)

	-- Add the T-SQL statements to compute the return value here
	SELECT	@name1 = h.PreferredName, 
			@cell1 = h.CellPhone,
			@name2 = s.PreferredName,
			@cell2 = s.CellPhone  
	FROM dbo.People p
	JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	LEFT JOIN dbo.People h ON f.FamilyId = h.FamilyId AND h.PeopleId = f.HeadOfHouseholdId
	LEFT JOIN dbo.People s ON f.FamilyId = s.FamilyId AND s.PeopleId = f.HeadOfHouseholdSpouseId
	WHERE p.PeopleId = @pid

	SET @names = @name1
	IF LEN(@name2) > 0
		SET @names = @name1 + ' and ' + @name2
	IF LEN(@cell1) > 0
		SET @names = @names + ', c ' + dbo.FmtPhone(@cell1)
	IF LEN(@cell2) > 0
		SET @names = @names + ', c ' + dbo.FmtPhone(@cell2)
	
	-- Return the result of the function
	RETURN @names

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
