IF object_id('GetDepositDateTotalsUsingAuthorizedFunds') IS NULL
BEGIN
EXEC ('
	create procedure dbo.GetDepositDateTotalsUsingAuthorizedFunds
	(
	  @authorizedFunds varchar(max),
	  @startDate datetime,
	  @endDate datetime
	) as
	begin
		declare @af table (
		  FundId int
		)

		insert into @af 
		select
		  v.Value
		from dbo.SplitInts(@authorizedFunds) v

		select
		  bh.DepositDate,
		  sum(isnull(bh.TotalCash, 0) + isnull(bh.TotalChecks, 0) + isnull(bh.TotalEnvelopes, 0)) as TotalHeader,
		  (
		  select 
			sum(isnull(c1.ContributionAmount, 0)) as expr1 
		  from dbo.Contribution c1
			inner join @af af on af.FundId = c1.FundId
			inner join dbo.BundleDetail bd1 on bd1.ContributionId = c1.ContributionId
			inner join dbo.BundleHeader bh1 on bh1.BundleHeaderId = bd1.BundleHeaderId
		  where bh1.DepositDate = bh.DepositDate 
		  ) as TotalContributions,
		  count(*) as [Count]
		from dbo.BundleHeader bh 
		where bh.DepositDate is not null
		and bh.DepositDate >= @startDate
		and bh.DepositDate <= @endDate
		group by
		  bh.DepositDate
	end')
END