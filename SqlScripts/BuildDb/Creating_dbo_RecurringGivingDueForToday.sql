CREATE VIEW [dbo].[RecurringGivingDueForToday]
AS 
WITH amt AS (
	SELECT ra.PeopleId, 
		Amt = SUM(ra.Amt) 
	FROM dbo.RecurringAmounts ra 
	JOIN dbo.ContributionFund f ON f.FundId = ra.FundId
	WHERE f.FundStatusId = 1
	AND f.OnlineSort IS NOT NULL
	GROUP BY ra.PeopleId
), VaultIds AS (
	SELECT PeopleId
		, VaultId = CASE 
				WHEN s.Setting = 'Sage' AND PreferredGivingType = 'b' THEN CONVERT(VARCHAR(50), SageBankGuid)
				WHEN s.Setting = 'Sage' AND PreferredGivingType = 'c' THEN CONVERT(VARCHAR(50), SageCardGuid)
				WHEN s.Setting = 'AuthorizeNet' THEN CONVERT(VARCHAR(50), AuNetCustId)
				WHEN s.Setting = 'Transnational' AND PreferredGivingType = 'b' THEN CONVERT(VARCHAR(50), TbnBankVaultId)
				WHEN s.Setting = 'Transnational' AND PreferredGivingType = 'c' THEN CONVERT(VARCHAR(50), TbnCardVaultId)
				END
	FROM dbo.PaymentInfo
	JOIN dbo.Setting s ON s.Id = 'TransactionGateway' AND NOT EXISTS(SELECT NULL FROM dbo.Setting WHERE id = 'TemporaryGatway')
)
SELECT 
	mg.PeopleId
	,p.Name2
	,a.Amt
FROM dbo.ManagedGiving mg
JOIN amt a ON a.PeopleId = mg.PeopleId
LEFT JOIN VaultIds v ON v.PeopleId = mg.PeopleId
JOIN dbo.People p ON p.PeopleId = mg.PeopleId
WHERE mg.NextDate = CONVERT(DATE, GETDATE())
AND a.amt > 0
AND (v.VaultId IS NOT NULL OR v.PeopleId = NULL)
AND NOT EXISTS(
	SELECT NULL 
	FROM dbo.[Transaction] t 
	WHERE t.LoginPeopleId = p.PeopleId 
	AND CONVERT(DATE, t.TransactionDate) = CONVERT(DATE, GETDATE())
	AND t.Approved = 0)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
