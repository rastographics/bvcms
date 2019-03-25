ALTER function [dbo].[Contributions2]
(
	@fd datetime, 
	@td datetime,
	@campusid int,
	@pledges bit,
	@nontaxded bit,
	@includeUnclosed bit
)
returns table 
as
return 
(
select 
	p.FamilyId,
	p.PeopleId,
    c.ContributionDate as [Date],
    
    case when fa.HeadOfHouseholdId = sp.PeopleId
			and isnull(sp.ContributionOptionsId, case when mssp.Married = 1 then 2 else 1 end) = 2
			and isnull(p.ContributionOptionsId, case when msp.Married = 1 then 2 else 1 end) = 2
		then sp.PeopleId 
		else c.PeopleId 
	end as CreditGiverId,

    case when isnull(sp.ContributionOptionsId, case when mssp.Married = 1 then 2 else 1 end) = 1
			or isnull(p.ContributionOptionsId, case when msp.Married = 1 then 2 else 1 end) = 1
		then null
		when fa.HeadOfHouseholdId = sp.PeopleId
		then c.PeopleId
		else sp.PeopleId
	end as CreditGiverId2,

    case when fa.HeadOfHouseholdId = sp.PeopleId
		then p.PeopleId 
		else sp.PeopleId 
	end as SpouseId,
	
    case when fa.HeadOfHouseholdId = sp.PeopleId
		then sp.Name2 
		else p.Name2 
	end as HeadName,
	
    case when fa.HeadOfHouseholdId = sp.PeopleId
		then p.Name2 
		else sp.Name2 
	end as SpouseName,
	
	case when c.ContributionTypeId <> 8
		then c.ContributionAmount
		else 0
	end as Amount,
	
	case when c.ContributionTypeId = 8
		then c.ContributionAmount
		else 0
	end as PledgeAmount,
	
    h.BundleHeaderId,
	c.ContributionDesc,
	c.CheckNo,
    c.FundId,
    f.FundName,
    case when f.FundPledgeFlag = 1 and f.FundStatusId = 1
		then 1
		else 0
	end as OpenPledgeFund,
    bht.[Description] as BundleType,
    bst.[Description] as BundleStatus,
    c.QBSyncID,
	c.ContributionId,
	c.[Source],
	c.MetaInfo,
	t.[Description] TransactionDesc,
	c.ContributionTypeId,
	ca.[Description] Campus
from dbo.Contribution c
	join dbo.ContributionFund f on c.FundId = f.FundId
	join dbo.BundleDetail d on c.ContributionId = d.ContributionId
	join dbo.BundleHeader h on d.BundleHeaderId = h.BundleHeaderId
	join lookup.BundleHeaderTypes bht on h.BundleHeaderTypeId = bht.Id
	join lookup.BundleStatusTypes bst on h.BundleStatusId = bst.Id
	join dbo.People p on c.PeopleId = p.PeopleId
	join dbo.Families fa on p.FamilyId = fa.FamilyId
	left join dbo.People sp on sp.PeopleId = p.SpouseId
	left join lookup.MaritalStatus mssp on mssp.Id = sp.MaritalStatusId
	left join lookup.MaritalStatus msp on msp.Id = p.MaritalStatusId
	left outer join dbo.[Transaction] t on t.Id = c.TranId
	left outer join lookup.Campus ca on ca.Id = c.CampusId
where c.ContributionTypeId not in (6,7) -- no reversed or returned
	and ((case when c.ContributionTypeId = 9 then 1 else isnull(f.NonTaxDeductible, 0) end) = @nontaxded or @nontaxded is null)
    and c.ContributionStatusId = 0 -- recorded
	and (c.ContributionTypeId <> 8 or isnull(@pledges, 1) = 1)
    and (@fd is null or c.ContributionDate >= @fd) and (@td is null or c.ContributionDate < dateadd(hh, 24, isnull(@td, convert(date,getdate()))))
	and (isnull(h.BundleStatusId, 0) = 0 or @includeUnclosed = 1)
    and (@campusid = 0 or c.CampusId = @campusid) -- campusid = 0 = all
)
GO


