CREATE VIEW [dbo].[ManagedGivingList] AS
SELECT 
	p.Name2,
	mg.PeopleId ,
    mg.StartWhen ,
    mg.NextDate ,
    mg.SemiEvery ,
    mg.Day1 ,
    mg.Day2 ,
    mg.EveryN ,
    mg.Period ,
    mg.StopWhen ,
    mg.StopAfter ,
    mg.type ,
	ActiveAmt = (SELECT SUM(Amt) 
			FROM  dbo.RecurringAmounts ra
			JOIN dbo.ContributionFund f ON f.FundId = ra.FundId
			WHERE PeopleId = mg.PeopleId
			AND f.OnlineSort IS NOT NULL 
			AND f.FundStatusId = 1),
	InactiveAmt = (SELECT SUM(Amt) 
			FROM  dbo.RecurringAmounts ra
			JOIN dbo.ContributionFund f ON f.FundId = ra.FundId
			WHERE PeopleId = mg.PeopleId
			AND (f.OnlineSort IS NULL OR f.FundStatusId = 2))
FROM dbo.ManagedGiving mg
JOIN dbo.People p ON p.PeopleId = mg.PeopleId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
