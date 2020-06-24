ALTER view [dbo].[RecurringGivingDueForToday]
as 
with amt as (
	select ra.PeopleId, SUM(ra.Amt) Amt
	from dbo.RecurringAmounts ra 
	join dbo.ContributionFund f on f.FundId = ra.FundId
	where f.FundStatusId = 1
	and f.OnlineSort is not null
	and not exists(SELECT 1 FROM dbo.Setting s WHERE s.Id = 'RecurringGiving' AND Setting = 'false')
	group by ra.PeopleId
),
VaultIds as (
	select pay.PeopleId,
		case
			when ga.GatewayId = 2 and PreferredGivingType = 'b' then convert(varchar(50), SageBankGuid)
			when ga.GatewayId = 2 and PreferredGivingType = 'c' then convert(varchar(50), SageCardGuid)
			when ga.GatewayId = 3 and PreferredGivingType = 'b' then convert(varchar(50), TbnBankVaultId)
			when ga.GatewayId = 3 and PreferredGivingType = 'c' then convert(varchar(50), TbnCardVaultId)
			when ga.GatewayId = 4 then convert(varchar(50), AuNetCustId)
			when ga.GatewayId = 5 then AcceptivaPayerId
		end AS VaultId
	from dbo.PaymentInfo pay
	join dbo.PaymentProcess pp on pp.ProcessName = 'Recurring Giving' and pay.GatewayAccountId = pp.GatewayAccountId
	join dbo.GatewayAccount ga on ga.GatewayAccountId = pp.GatewayAccountId
),
Managed as (
select 
	mg.PeopleId
	,p.Name2
	,a.Amt
from dbo.ManagedGiving mg
join amt a on a.PeopleId = mg.PeopleId
left join VaultIds v on v.PeopleId = mg.PeopleId
join dbo.People p on p.PeopleId = mg.PeopleId
where mg.NextDate = convert(date, getdate())
and a.amt > 0
and (v.VaultId is not null or v.PeopleId is null)
and not exists(
	select null 
	from dbo.[Transaction] t 
	where t.LoginPeopleId = p.PeopleId 
	and convert(date, t.TransactionDate) = convert(date, getdate())
	and t.Approved = 0)
),
Scheduled as (
	select p.PeopleId, p.Name2, sum(sa.Amount) Amt
	from dbo.ScheduledGift sg
	join dbo.ScheduledGiftAmount sa on sa.ScheduledGiftId = sg.ScheduledGiftId
	join dbo.People p on p.PeopleId = sg.PeopleId
	WHERE sg.IsEnabled = 1 
	and sg.StartDate >= getdate()
	and (sg.EndDate IS NULL or sg.EndDate > getdate())
	and convert(date, sg.NextOccurrence) = convert(date, getdate())
	group by p.PeopleId, p.Name2
)
SELECT * from Managed UNION ALL
SELECT * from Scheduled
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SettingMetadata WHERE SettingId = 'RecurringGiving')
BEGIN

DECLARE @Contributions INT = 3, @dataBool INT = 1;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId, DefaultValue) VALUES
    ('RecurringGiving', 'When enabled, recurring giving is processed automatically.', @dataBool, @Contributions, 'true')
END
GO
