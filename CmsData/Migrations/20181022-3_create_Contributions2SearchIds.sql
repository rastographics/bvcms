IF OBJECT_ID('dbo.Contributions2SearchIds') IS NOT NULL
	DROP FUNCTION dbo.Contributions2SearchIds
GO

CREATE FUNCTION [dbo].[Contributions2SearchIds] ( @json varchar(max) )
RETURNS 
@t table ( ContributionId int )
AS
begin
	declare @MinDate date = (select json_value(@json, '$.MinDate'))
	declare @MaxDate date = (select json_value(@json, '$.MaxDate'))
	declare @CampusId int = (select json_value(@json, '$.CampusId'))
	declare @NonTaxDed int = isnull((select json_value(@json, '$.NonTaxDed')), 0) -- default 0 = taxdeductible
	declare @BundleTypes varchar(max) = (select json_value(@json, '$.BundleTypes'))
	declare @FundIds varchar(max) = (select json_value(@json, '$.FundIds'))
	declare @Description varchar(max) = (select json_value(@json, '$.Description'))
	declare @TransactionDesc varchar(max) = (select json_value(@json, '$.TransactionDesc'))
	declare @Source int = (select json_value(@json, '$.Source'))
	declare @ContributionTags varchar(max) = (select json_value(@json, '$.ContributionTags'))

	declare @notfunds bit = 0
	if @FundIds like '<>%'
	begin
		set @notfunds = 1
		set @FundIds = substring(@FundIds, 3, 3000)
	end
	declare @funds table (fundid int)
	insert @funds ( fundid )
	select value from dbo.SplitInts(@FundIds)

	declare @notdesc bit = 0
	if @Description like '<>%'
	begin
		set @notdesc = 1
		set @Description = substring(@Description, 3, 100)
	end

	declare @nottrandesc bit = 0
	if @TransactionDesc like '<>%'
	begin
		set @nottrandesc = 1
		set @TransactionDesc = substring(@TransactionDesc, 3, 100)
	end

	declare @nottags bit = 0
	if @ContributionTags like '<>%'
	begin
		set @nottags = 1
		set @ContributionTags = substring(@ContributionTags, 3, 3000)
	end
	declare @tags table (TagName varchar(50))
	insert @tags ( TagName )
	select value from dbo.Split(@ContributionTags, ',')

	declare @btypes table (btype varchar(50))
	insert @btypes ( btype )
	select value from dbo.Split(@BundleTypes, ',')

	insert @t ( ContributionId )
	select ContributionId
	from dbo.contributions2(@MinDate, @MaxDate 
		,isnull(@CampusId, 0) /* 0 = any campus */
		,0 /* no pledges */
		,iif(@NonTaxDed = -1, null, @NonTaxDed) /* -1 = either, 0 = taxded (default), 1 = nontaxded */
		,1 /* include unclosed bundles */ 
	) c
	where (@BundleTypes is null or exists(select null from @btypes where btype = c.BundleType))
	and case when @FundIds is null then 1
			when @notfunds = 1 then iif(not exists(select null from @funds where fundid = c.FundId), 1, 0)
			else iif(exists(select null from @funds where fundid = c.FundId), 1, 0)
			end = 1
	and case when @Description is null then 1
			when @notdesc = 1 then iif(c.ContributionDesc <> @Description, 1, 0)
			else iif(c.ContributionDesc = @Description, 1, 0)
			end = 1
	and case when @TransactionDesc is null then 1
			when @nottrandesc = 1 then iif(c.TransactionDesc <> @TransactionDesc, 1, 0)
			else iif(c.TransactionDesc = @TransactionDesc, 1, 0)
			end = 1
	and (@Source is null or isnull(c.Source, 0) = @Source) /* 0 = not mobile, 1 = mobile */
	and case when @ContributionTags is null then 1
			when @nottags = 1 then iif(not exists(select null from dbo.ContributionTag ct join @tags tt on tt.TagName = ct.TagName and ct.ContributionId = c.ContributionId), 1, 0)
			else iif(exists(select null from dbo.ContributionTag ct join @tags tt on tt.TagName = ct.TagName and ct.ContributionId = c.ContributionId), 1, 0)
			end = 1
	return
end
