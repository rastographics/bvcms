CREATE FUNCTION [dbo].[FamilyGiver](@fd DATETIME, @td DATETIME, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
WITH units AS ( 
	SELECT c.FamilyId
           ,SUM(c.Amount) Amount
           ,SUM(c.PledgeAmount) Pledge
	FROM     dbo.Contributions2(@fd, @td, 0, 1, NULL, 1) c
	WHERE ISNULL(@fundid, 0) = 0 OR c.FundId = @fundid
	GROUP BY c.FamilyId
)
SELECT  p.FamilyId
       ,p.PeopleId
       ,FamGive = CAST(IIF(u.Amount > 0, 1, 0) AS BIT)
       ,FamPledge = CAST(IIF(u.Pledge > 0, 1, 0) AS BIT)
FROM dbo.People p
LEFT JOIN units u ON u.FamilyId = p.FamilyId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
