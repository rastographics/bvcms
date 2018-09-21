CREATE VIEW [export].[XpContribution] AS 
SELECT c.ContributionId ,
       PeopleId ,
       BundleType = bt.[Description],
       bh.DepositDate ,
       Fund = f.FundName ,
       [Type] = ct.[Description] ,
       [Date] = c.ContributionDate ,
       Amount =  c.ContributionAmount,
       [Description] = ContributionDesc ,
       [Status] = cs.[Description],
       Pledge = c.PledgeFlag,
       CheckNo ,
       Campus = ca.[Description]
FROM dbo.Contribution c
JOIN dbo.BundleDetail bd ON bd.ContributionId = c.ContributionId
JOIN dbo.BundleHeader bh ON bh.BundleHeaderId = bd.BundleHeaderId
LEFT JOIN lookup.BundleHeaderTypes bt ON bt.Id = bh.BundleHeaderTypeId
LEFT JOIN dbo.ContributionFund f ON f.FundId = c.FundId
LEFT JOIN lookup.ContributionStatus cs ON cs.Id = c.ContributionStatusId
LEFT JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
LEFT JOIN lookup.Campus ca ON ca.Id = CampusId

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
