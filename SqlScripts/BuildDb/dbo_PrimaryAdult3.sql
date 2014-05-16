CREATE PROCEDURE [dbo].[PrimaryAdult3]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT PeopleId, v.cnt, v.FamilyId FROM dbo.People p
	JOIN (SELECT COUNT(*) cnt, FamilyId
	FROM dbo.People
	WHERE PositionInFamilyId = 10
	GROUP BY FamilyId
	HAVING COUNT(*) > 2) v
	ON p.FamilyId = v.FamilyId
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
