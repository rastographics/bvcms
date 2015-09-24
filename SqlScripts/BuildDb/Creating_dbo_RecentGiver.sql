CREATE FUNCTION [dbo].[RecentGiver](@days INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
		SELECT *
		FROM Contributions2(
				DATEADD(DAY, -@days, GETDATE()),
				GETDATE(),
				0, 0, 0, 1)
		WHERE Amount > 0
	), BOTH AS (
		SELECT contributions.CreditGiverId PeopleId
		FROM contributions

		UNION

		SELECT contributions.SpouseId 
		FROM contributions
	)
	SELECT DISTINCT PeopleId FROM BOTH
	WHERE PeopleId IS NOT NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
