CREATE FUNCTION [dbo].[CheckinMatch]( @id nvarchar(20) )
RETURNS 
@t TABLE 
(
	 familyid INT
	,areacode nvarchar(3)
	,NAME nvarchar(100)
	,phone nvarchar(20)
	,locked BIT
)
AS
BEGIN
	SET @id = dbo.GetDigits(@id)
	DECLARE @ph nvarchar(10) = REPLACE(STR(@id, 10), SPACE(1), '0') 
	DECLARE @p7 nvarchar(7) = SUBSTRING(@ph, 4, 7) + '%'
	DECLARE @ac nvarchar(4) = SUBSTRING(@ph, 1, 3)

	DECLARE @tpins TABLE ( fid INT )
	INSERT @tpins SELECT f.FamilyId 
	FROM dbo.PeopleExtra e 
	JOIN dbo.People p ON e.PeopleId = p.PeopleId
	JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	WHERE e.Field = 'PIN' AND e.Data = @id
	
	DECLARE @cells TABLE(fid INT)
	INSERT @cells SELECT p.FamilyId
	FROM dbo.People p 
	WHERE p.CellPhoneLU LIKE @p7
	AND (@ac = '000' OR @ac = p.CellPhoneAC)
	
	DECLARE @locks TABLE ( fid INT )
	INSERT @locks
		SELECT FamilyId FROM dbo.FamilyCheckinLock WHERE DATEDIFF(s, Created, GETDATE()) < 60 AND Locked = 1
		
	INSERT INTO @t SELECT
		f.FamilyId,
		f.HomePhoneAC AreaCode,
		hh.Name,
		@id phone,
		CAST(CASE WHEN lo.fid IS NOT NULL THEN 1 ELSE 0 END AS bit) locked
	FROM dbo.Families f
	JOIN dbo.People hh ON f.HeadOfHouseholdId = hh.PeopleId AND hh.DeceasedDate IS NULL
	LEFT JOIN @locks lo ON fid = f.FamilyId
	WHERE (f.HomePhoneLU LIKE @p7 AND (@ac = '000' OR @ac = f.HomePhoneAC))
			OR EXISTS(SELECT NULL FROM @cells WHERE fid = f.FamilyId)
			OR EXISTS(SELECT NULL FROM @tpins WHERE fid = f.FamilyId)
		
	RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
