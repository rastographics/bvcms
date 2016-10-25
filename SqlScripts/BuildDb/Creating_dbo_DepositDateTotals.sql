CREATE VIEW [dbo].[DepositDateTotals] AS
(
	SELECT 
		 b.DepositDate 
		,TotalHeader = SUM(ISNULL(b.TotalCash,0)) + SUM(ISNULL(b.TotalChecks, 0)) + SUM(ISNULL(b.TotalEnvelopes, 0))
		,TotalContributions = (
			SELECT SUM(ISNULL(ContributionAmount, 0))
			FROM dbo.Contribution c
			JOIN dbo.BundleDetail d ON d.ContributionId = c.ContributionId
			JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
			WHERE h.DepositDate = b.DepositDate)
		,[Count] = COUNT(*)
	FROM dbo.BundleHeader b
	WHERE b.DepositDate IS NOT NULL
	GROUP BY DepositDate
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
