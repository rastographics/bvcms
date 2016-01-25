CREATE VIEW [dbo].[FailedRecurringGiving] AS
WITH failedb AS (
	SELECT 
		PeopleId
		, MAX(ContributionId) ContributionId
	FROM dbo.Contribution
	WHERE TranId IS NOT NULL
	AND ContributionDesc = 'Recurring Giving'
	AND ContributionStatusId > 0
	GROUP BY PeopleId
)
, failedc AS (
	SELECT tp.PeopleId, MAX(t.TransactionDate) TransactionDate
	FROM dbo.[Transaction] t
	JOIN dbo.TransactionPeople tp ON tp.Id = t.Id
	WHERE Description = 'Recurring Giving'
	AND Approved = 0
	GROUP BY tp.PeopleId
)
, failed AS (
	SELECT  
		p.PeopleId
		, Dt = ISNULL(cc.TransactionDate, c.ContributionDate)
	FROM dbo.People p
	LEFT JOIN failedc cc ON cc.PeopleId = p.PeopleId
	LEFT JOIN failedb bc ON bc.PeopleId = p.PeopleId
	LEFT JOIN dbo.Contribution c ON c.ContributionId = bc.ContributionId
	WHERE bc.PeopleId IS NOT NULL OR cc.PeopleId IS NOT NULL
)
SELECT * FROM failed


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
