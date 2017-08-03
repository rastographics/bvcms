CREATE VIEW [dbo].[ChAiGiftData]
AS
SELECT c.PeopleId
	,p.FamilyId
	,c.ContributionId
	,c.CreatedDate
	,c.ContributionDate
	,c.ContributionAmount
	,Type = ct.Description
	,Status = cs.Description
	,BundleType = ht.Description
	,f.FundName
	,PaymentType = CASE t.PaymentType WHEN 'C' THEN 'Card' WHEN 'B' THEN 'ACH' ELSE 'Plate' END
	,c.CheckNo
FROM dbo.Contribution c
JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
JOIN lookup.ContributionStatus cs ON cs.Id = c.ContributionStatusId
JOIN dbo.BundleDetail d ON d.ContributionId = c.ContributionId
JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
JOIN lookup.BundleHeaderTypes ht ON ht.Id = h.BundleHeaderTypeId
JOIN dbo.ContributionFund f ON f.FundId = c.FundId
JOIN dbo.People p ON p.PeopleId = c.PeopleId
LEFT JOIN dbo.[Transaction] t ON c.TranId = t.Id
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
