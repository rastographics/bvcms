CREATE FUNCTION [dbo].[GetContributionTotalsBothIfJoint](@startdt DATETIME, @enddt DATETIME)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		PeopleId, 
		Name, 
		tt.Amount, 
		CASE WHEN CreditGiverId = PeopleId THEN 1 ELSE 0 END CreditedGiver  
	FROM
	(
		SELECT 
			CreditGiverId PeopleId, 
			HeadName Name, 
			Amount, 
			CreditGiverId
		FROM dbo.GetTotalContributionsDonor2(@startdt, @enddt, 0, 0, 1) 

		UNION

		SELECT 
			SpouseId PeopleId, 
			SpouseName Name, 
			Amount, 
			CreditGiverId
		FROM dbo.GetTotalContributionsDonor2('1/1/14', '12/31/14', 0, 0, 1) 
		WHERE SpouseId IS NOT NULL AND SpouseId <> CreditGiverId
	) tt
)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
