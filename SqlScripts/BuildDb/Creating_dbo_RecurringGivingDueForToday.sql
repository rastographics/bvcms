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
)
SELECT 
	mg.PeopleId
	,p.Name2
	,a.Amt
FROM dbo.ManagedGiving mg
JOIN amt a ON a.PeopleId = mg.PeopleId
JOIN dbo.People p ON p.PeopleId = mg.PeopleId
WHERE mg.NextDate = CONVERT(DATE, GETDATE())
AND a.amt > 0
AND NOT EXISTS(
	SELECT NULL 
	FROM dbo.[Transaction] t 
	WHERE t.LoginPeopleId = p.PeopleId 
	AND CONVERT(DATE, t.TransactionDate) = CONVERT(DATE, GETDATE())
	AND t.Approved = 0)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
