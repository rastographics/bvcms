CREATE view [dbo].[RecurringGivingDueForToday]
as 
with amt as (
	select ra.PeopleId, 
		Amt = sum(ra.Amt) 
	from dbo.RecurringAmounts ra 
	join dbo.ContributionFund f on f.FundId = ra.FundId
	where f.FundStatusId = 1
	and f.OnlineSort is not null
	group by ra.PeopleId
), VaultIds as (
	select PeopleId
		, VaultId = case 
				when s.Setting = 'Sage' and PreferredGivingType = 'b' then convert(varchar(50), SageBankGuid)
				when s.Setting = 'Sage' and PreferredGivingType = 'c' then convert(varchar(50), SageCardGuid)
				when s.Setting = 'AuthorizeNet' then convert(varchar(50), AuNetCustId)
				when s.Setting = 'Transnational' and PreferredGivingType = 'b' then convert(varchar(50), TbnBankVaultId)
				when s.Setting = 'Transnational' and PreferredGivingType = 'c' then convert(varchar(50), TbnCardVaultId)
				end
	from dbo.PaymentInfo
	join dbo.Setting s on s.Id = 'TransactionGateway' and not exists(select null from dbo.Setting where id = 'TemporaryGatway')
)
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

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
