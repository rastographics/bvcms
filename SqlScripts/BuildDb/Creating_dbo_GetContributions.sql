-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetContributions](@fid INT, @pledge bit)
RETURNS TABLE 
AS
RETURN 
(
SELECT p.PeopleId, p.PreferredName First, sp.PreferredName Spouse, p.LastName LAST, p.PrimaryAddress Addr, p.PrimaryCity City, p.PrimaryState ST, p.PrimaryZip Zip, MAX(ContributionDate) ContributionDate, SUM(ContributionAmount) Amt
FROM dbo.Contribution c
JOIN dbo.ContributionFund f ON c.FundId = f.FundId
JOIN dbo.People p ON c.PeopleId = p.PeopleId
LEFT JOIN dbo.People sp ON p.SpouseId = sp.PeopleId
WHERE (CASE WHEN c.ContributionTypeId = 8 THEN 1 ELSE 0 END) = @pledge AND c.ContributionAmount > 0 AND f.fundid = @fid
GROUP BY P.PeopleId, p.LastName, p.PreferredName, sp.PreferredName, p.PrimaryAddress, p.PrimaryCity, p.PrimaryState, p.PrimaryZip
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
