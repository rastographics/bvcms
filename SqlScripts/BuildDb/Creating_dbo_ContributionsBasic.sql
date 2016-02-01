
CREATE VIEW [dbo].[ContributionsBasic] AS
SELECT c.ContributionId ,
       c.PeopleId ,
	   p.FamilyId,
       c.ContributionAmount ,
       c.ContributionDate ,
       c.CheckNo ,
       c.ContributionTypeId ,
	   c.FundId,
	   h.BundleHeaderTypeId
FROM dbo.Contribution c
JOIN dbo.People p ON p.PeopleId = c.PeopleId
JOIN dbo.BundleDetail d ON d.ContributionId = c.ContributionId
JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
WHERE ISNULL(c.PledgeFlag, 0) = 0
AND c.ContributionStatusId = 0
AND c.ContributionTypeId NOT IN (6,7,8)

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
