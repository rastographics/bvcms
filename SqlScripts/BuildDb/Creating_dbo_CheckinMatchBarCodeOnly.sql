CREATE FUNCTION [dbo].[CheckinMatchBarCodeOnly]( @id nvarchar(20) )
RETURNS 
@t TABLE 
(
	 familyid INT
	,NAME nvarchar(100)
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
	
	DECLARE @locks TABLE ( fid INT )
	INSERT @locks
		SELECT FamilyId FROM dbo.FamilyCheckinLock WHERE DATEDIFF(s, Created, GETDATE()) < 60 AND Locked = 1
		
	INSERT INTO @t SELECT
		f.FamilyId,
		hh.Name,
		CAST(CASE WHEN lo.fid IS NOT NULL THEN 1 ELSE 0 END AS bit) locked
	FROM dbo.Families f
	JOIN dbo.People hh ON f.HeadOfHouseholdId = hh.PeopleId AND hh.DeceasedDate IS NULL
	LEFT JOIN @locks lo ON fid = f.FamilyId
	WHERE EXISTS(SELECT NULL FROM @tpins WHERE fid = f.FamilyId)
			
	RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
