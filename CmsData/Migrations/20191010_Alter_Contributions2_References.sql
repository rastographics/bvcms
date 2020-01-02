ALTER FUNCTION [dbo].[Contributions0]
(
	@fd DATETIME, 
	@td DATETIME,
	@fundid INT,
	@campusid INT,
	@pledges BIT,
	@nontaxded BIT,
	@includeUnclosed BIT
)
RETURNS 
@t TABLE ( PeopleId INT )
AS
BEGIN
	DECLARE @cc TABLE (PeopleId INT)
	INSERT INTO @cc
	SELECT CreditGiverId 
	FROM Contributions2(@fd, @td, @campusid, @pledges, @nontaxded, @includeUnclosed, null)
	UNION
	SELECT CreditGiverId2 
	FROM Contributions2(@fd, @td, @campusid, @pledges, @nontaxded, @includeUnclosed, null)
	WHERE CreditGiverId2 IS NOT NULL

	INSERT @t ( PeopleId )
	SELECT p.PeopleId 
	FROM dbo.People p
	LEFT JOIN @cc c ON c.PeopleId = p.PeopleId
	WHERE c.PeopleId IS NULL
	
	RETURN 
END
GO

ALTER function [dbo].[Contributions2SearchIds] ( @json varchar(max) )
returns 
@t table ( ContributionId int )
as
begin
	declare @MinDate date = (select json_value(@json, '$.MinDate'))
	declare @MaxDate date = (select json_value(@json, '$.MaxDate'))
	declare @CampusId int = (select json_value(@json, '$.CampusId'))
	declare @NonTaxDed int = isnull((select json_value(@json, '$.NonTaxDed')), 0) -- default 0 = taxdeductible
	declare @BundleTypes varchar(max) = (select json_value(@json, '$.BundleTypes'))
	declare @FundIds varchar(max) = (select json_value(@json, '$.FundIds'))
	declare @ContributionDesc varchar(max) = (select json_value(@json, '$.ContributionDesc'))
	declare @TransactionDesc varchar(max) = (select json_value(@json, '$.TransactionDesc'))
	declare @Source int = (select json_value(@json, '$.Source'))
	declare @ContributionTags varchar(max) = (select json_value(@json, '$.ContributionTags'))
	declare @FromAmount int = (select json_value(@json, '$.FromAmount'))
	declare @ToAmount int = (select json_value(@json, '$.ToAmount')) --up to, not including

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
	if @ContributionDesc like '<>%'
	begin
		set @notdesc = 1
		set @ContributionDesc = substring(@ContributionDesc, 3, 100)
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
		, null
	) c
	where (@BundleTypes is null or exists(select null from @btypes where btype = c.BundleType))
	and case when @FundIds is null then 1
			 when @notfunds = 1 then iif(not exists(select null from @funds where fundid = c.FundId), 1, 0)
			 else iif(exists(select null from @funds where fundid = c.FundId), 1, 0)
			 end = 1
	and case when @ContributionDesc is null then 1
			 when charindex('%', @ContributionDesc) > 0 and @notdesc = 1 then iif(isnull(c.ContributionDesc, '') not like @ContributionDesc, 1, 0)
			 when charindex('%', @ContributionDesc) > 0 then iif(isnull(c.ContributionDesc, '') like @ContributionDesc, 1, 0)
			 when @notdesc = 1 then iif(isnull(c.ContributionDesc, '') <> @ContributionDesc, 1, 0)
			 else iif(c.ContributionDesc = @ContributionDesc, 1, 0)
			 end = 1
	and case when @TransactionDesc is null then 1
			 when @nottrandesc = 1 then iif(isnull(c.TransactionDesc, '') <> @TransactionDesc, 1, 0)
			 else iif(c.TransactionDesc = @TransactionDesc, 1, 0)
			 end = 1
	and case when @Source is null then 1
			 when @Source = 0 then iif(isnull(c.[source], 0) = 0, 1, 0) -- Source <> 1 (null or 0) (not mobile)
			 else iif(c.[Source] = @Source, 1, 0) -- Source = 1 (mobile)
             end = 1
	and case when @FromAmount is null then 1
			else iif(c.Amount >= @FromAmount, 1, 0)
			end = 1
	and case when @ToAmount is null then 1
			else iif(c.Amount < @ToAmount, 1, 0)
			end = 1
	and case when @ContributionTags is null then 1
			 when charindex('%', @ContributionTags) > 0 and @nottags = 1 
				then iif(not exists(select null from dbo.ContributionTag ct 
					where ct.TagName like @ContributionTags and ct.ContributionId = c.ContributionId)
					, 1, 0)
			 when charindex('%', @ContributionTags) > 0 
				then iif(exists(select null from dbo.ContributionTag ct 
					where ct.TagName like @ContributionTags and ct.ContributionId = c.ContributionId)
					, 1, 0)
			 when @nottags = 1 
				then iif(not exists(select null from dbo.ContributionTag ct 
					join @tags tt on tt.TagName = @ContributionTags and ct.ContributionId = c.ContributionId)
					, 1, 0)
			 else 
				iif(exists(select null from dbo.ContributionTag ct 
					join @tags tt on tt.TagName = @ContributionTags and ct.ContributionId = c.ContributionId)
					, 1, 0)
			 end = 1
	return
end

GO

ALTER FUNCTION [dbo].[FamilyGiver](@fd DATETIME, @td DATETIME, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
WITH units AS ( 
	SELECT c.FamilyId
           ,SUM(c.Amount) Amount
           ,SUM(c.PledgeAmount) Pledge
	FROM     dbo.Contributions2(@fd, @td, 0, 1, NULL, 1, null) c
	WHERE ISNULL(@fundid, 0) = 0 OR c.FundId = @fundid
	GROUP BY c.FamilyId
)
SELECT  p.FamilyId
       ,p.PeopleId
       ,FamGive = CAST(IIF(u.Amount > 0, 1, 0) AS BIT)
       ,FamPledge = CAST(IIF(u.Pledge > 0, 1, 0) AS BIT)
FROM dbo.People p
LEFT JOIN units u ON u.FamilyId = p.FamilyId
)
GO

ALTER FUNCTION [dbo].[FamilyGiverFunds](@fd DATETIME, @td DATETIME, @funds varchar(max))
RETURNS TABLE 
AS
RETURN 
(
WITH units AS ( 
	SELECT c.FamilyId
           ,SUM(c.Amount) Amount
           ,SUM(c.PledgeAmount) Pledge
	FROM     dbo.Contributions2(@fd, @td, 0, 1, NULL, 1, null) c
	WHERE FundId in (select value from dbo.SplitInts(@funds))
	GROUP BY c.FamilyId
)
SELECT  p.FamilyId
       ,p.PeopleId
       ,FamGive = CAST(IIF(u.Amount > 0, 1, 0) AS BIT)
       ,FamPledge = CAST(IIF(u.Pledge > 0, 1, 0) AS BIT)
FROM dbo.People p
LEFT JOIN units u ON u.FamilyId = p.FamilyId
)
GO

ALTER FUNCTION [dbo].[FirstTimeGivers] ( @days INT, @fundid INT )
RETURNS TABLE 
AS
RETURN 
	SELECT PeopleId, FirstDate, Amt
	FROM
	(
		SELECT CreditGiverId, SUM(Amount) AS Amt, MIN(Date) as FirstDate
		FROM dbo.Contributions2('1/1/1900', GETDATE(), 0, 0, 0, 1, null) c
		WHERE c.FundId = @fundid OR @fundid = 0
		GROUP BY CreditGiverId
	) tt
	JOIN dbo.People p ON p.PeopleId = tt.CreditGiverId
	WHERE FirstDate > DATEADD(dd, -@days, GETDATE())
GO

ALTER FUNCTION [dbo].[GetTotalContributions2]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		CreditGiverId, 
		HeadName,
		SpouseName,
		[Count],
		Amount,
		PledgeAmount,
		FundId,
		FundName,
		[OnLine],
		QBSynced,
		o.OrganizationName MainFellowship, 
		ms.Description MemberStatus, 
		bht.Description BundleType 
	FROM
	(
	SELECT 
		CreditGiverId, 
		HeadName, 
		SpouseName, 
		COUNT(*) AS [Count], 
		SUM(Amount) AS Amount, 
		SUM(PledgeAmount) AS PledgeAmount, 
		c2.FundId, 
		FundName,
		BundleHeaderTypeId,
		CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END AS [OnLine],
		CASE WHEN QBSyncID IS NULL THEN 0 ELSE 1 END QBSynced
	FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed, null) c2
	JOIN dbo.BundleHeader h ON c2.BundleHeaderId = h.BundleHeaderId
	GROUP BY CreditGiverId, HeadName, SpouseName, c2.FundId, FundName, 
		CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END,
		CASE WHEN QBSyncID IS NULL THEN 0 ELSE 1 END,
		BundleHeaderTypeId
	) tt 
	JOIN dbo.People p ON p.PeopleId = tt.CreditGiverId
	JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	JOIN lookup.BundleHeaderTypes bht ON tt.BundleHeaderTypeId = bht.Id
	LEFT OUTER JOIN dbo.Organizations o ON o.OrganizationId = p.BibleFellowshipClassId
)
GO

ALTER FUNCTION [dbo].[GetTotalContributions3]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT tt.*, 
	(SELECT TOP 1 OrganizationName from dbo.Organizations o WHERE o.OrganizationId = p.BibleFellowshipClassId) MainFellowship,
	(SELECT TOP 1 Description FROM lookup.MemberStatus ms WHERE p.MemberStatusId = ms.Id) MemberStatus,
	p.JoinDate
	FROM
	(
	SELECT 
		CreditGiverId, 
		(HeadName + CASE WHEN SpouseId <> CreditGiverId THEN '*' ELSE '' END) HeadName, 
		(SpouseName + CASE WHEN SpouseId = CreditGiverId THEN '*' ELSE '' END) SpouseName, 
		COUNT(*) AS [Count], 
		SUM(Amount) AS Amount, 
		SUM(PledgeAmount) AS PledgeAmount, 
		c2.FundId, 
		FundName
	FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed, null) c2
	GROUP BY CreditGiverId, HeadName, SpouseId, SpouseName, SpouseId, c2.FundId, FundName
	) tt 
	JOIN dbo.People p ON p.PeopleId = tt.CreditGiverId
)
GO

ALTER FUNCTION [dbo].[GetTotalPledgesDonor2]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@pledgefund INT
)
RETURNS TABLE
AS
RETURN 
(
	WITH contributions AS ( 
		SELECT 
			CreditGiverId, 
			CreditGiverId2,
			HeadName, 
			SpouseName, 
			COUNT(*) AS [Count], 
			SUM(PledgeAmount) AS PledgeAmount,
			SUM(Amount) AS Amount
		FROM dbo.Contributions2(@fd, @td, @campusid, NULL, NULL, 1, null)
		WHERE ISNULL(@pledgefund, 0) = 0 OR FundId = @pledgefund
		GROUP BY CreditGiverId, CreditGiverId2, HeadName, SpouseName
	)
	SELECT 
		c.CreditGiverId, 
		c.CreditGiverId2,
		c.HeadName,
		c.SpouseName,
		c.[Count],
		c.PledgeAmount,
		c.Amount,
		Balance = IIF(c.PledgeAmount > 0, c.PledgeAmount - ISNULL(c.Amount, 0), 0),
		MainFellowship = ISNULL(o.OrganizationName, ''), 
		MemberStatus = ms.[Description], 
		p.JoinDate, 
		p.SpouseId, 
		[Option] = op.[Description],
		Addr = p.PrimaryAddress,
		Addr2 = p.PrimaryAddress2,
		City = p.PrimaryCity,
		ST = p.PrimaryState,
		Zip = p.PrimaryZip
	FROM contributions c
	JOIN dbo.People p ON p.PeopleId = c.CreditGiverId
	JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	LEFT JOIN lookup.EnvelopeOption op ON op.Id = p.ContributionOptionsId
	LEFT OUTER JOIN dbo.Organizations o ON o.OrganizationId = p.BibleFellowshipClassId
)
GO

ALTER FUNCTION [dbo].[GivingChange] (@days INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH period1 AS (
		SELECT p1.PeopleId, SUM(c1.Amount) c1amt, 0 c2amt
		FROM dbo.People p1
		JOIN dbo.Contributions2(
			DATEADD(DAY, -@days * 2, GETDATE()) -- start date
			,DATEADD(DAY, -@days, GETDATE()) -- enddate
			,0, 0, 0, 1, null) c1 ON p1.PeopleId = c1.CreditGiverId
		GROUP BY p1.PeopleId
	), 
	period2 AS (
		SELECT p2.PeopleId, 0 c1amt, SUM(c2.Amount) c2amt
		FROM dbo.People p2
		JOIN dbo.Contributions2(
			DATEADD(DAY, -@days, GETDATE()) -- start date
			,GETDATE() -- end date
			,0, 0, 0, 1, null) c2 ON p2.PeopleId = c2.CreditGiverId
		GROUP BY p2.PeopleId
	), 
	bothperiods AS (
		SELECT PeopleId, c1amt, c2amt FROM period1
		UNION
		SELECT PeopleId, c1amt, c2amt FROM period2
	),
	grouped AS (
		SELECT 
			PeopleId 
			,TotalPeriod1 = ISNULL(SUM(c1amt), 0)
			,TotalPeriod2 = ISNULL(SUM(c2amt), 0)
		FROM bothperiods GROUP BY PeopleId
	), 
	getpercent AS (
		SELECT 
			PeopleId, TotalPeriod1, TotalPeriod2, 
			PctOfPrevious = (NULLIF(TotalPeriod2, 0) / NULLIF(TotalPeriod1, 0)) * 100
		FROM grouped
	) 
	SELECT 
		PeopleId, 
		TotalPeriod1, 
		TotalPeriod2, 
		PctChange = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 100000.0
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN -100000.0
			ELSE PctOfPrevious - 100.0
		END,
		Change = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 'Started'
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN 'Stopped'
			WHEN PctOfPrevious > 100 THEN FORMAT(-(TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Increase'
			WHEN PctOfPrevious < 100 THEN FORMAT((TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Decrease'
			ELSE 'No Change'
		END
	FROM getpercent
)
GO

ALTER FUNCTION [dbo].[GivingChangeFund] (@days INT, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH period1 AS (
		SELECT p1.PeopleId, SUM(c1.Amount) c1amt, 0 c2amt
		FROM dbo.People p1
		JOIN dbo.Contributions2(
			DATEADD(DAY, -@days * 2, GETDATE()) -- start date
			,DATEADD(DAY, -@days, GETDATE()) -- enddate
			,0, 0, 0, 1, null) c1 ON p1.PeopleId = c1.CreditGiverId AND c1.FundId = @fundid
		GROUP BY p1.PeopleId
	), 
	period2 AS (
		SELECT p2.PeopleId, 0 c1amt, SUM(c2.Amount) c2amt
		FROM dbo.People p2
		JOIN dbo.Contributions2(
			DATEADD(DAY, -@days, GETDATE()) -- start date
			,GETDATE() -- end date
			,0, 0, 0, 1, null) c2 ON p2.PeopleId = c2.CreditGiverId AND c2.FundId = @fundid
		GROUP BY p2.PeopleId
	), 
	bothperiods AS (
		SELECT PeopleId, c1amt, c2amt FROM period1
		UNION
		SELECT PeopleId, c1amt, c2amt FROM period2
	),
	grouped AS (
		SELECT 
			PeopleId 
			,TotalPeriod1 = ISNULL(SUM(c1amt), 0)
			,TotalPeriod2 = ISNULL(SUM(c2amt), 0)
		FROM bothperiods GROUP BY PeopleId
	), 
	getpercent AS (
		SELECT 
			PeopleId, TotalPeriod1, TotalPeriod2, 
			PctOfPrevious = (NULLIF(TotalPeriod2, 0) / NULLIF(TotalPeriod1, 0)) * 100
		FROM grouped
	) 
	SELECT 
		f.FundName,
		PeopleId, 
		TotalPeriod1, 
		TotalPeriod2, 
		PctChange = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 100000.0
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN -100000.0
			ELSE PctOfPrevious - 100.0
		END,
		Change = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 'Started'
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN 'Stopped'
			WHEN PctOfPrevious > 100 THEN FORMAT(-(TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Increase'
			WHEN PctOfPrevious < 100 THEN FORMAT((TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Decrease'
			ELSE 'No Change'
		END
	FROM getpercent pc
	JOIN dbo.ContributionFund f ON f.FundId = @fundid
)
GO

ALTER FUNCTION [dbo].[GivingChangeFundQuarters] (@fd DATETIME, @td DATETIME, @fundids VARCHAR(MAX), @tagid INT)
RETURNS TABLE 
AS
RETURN 
(
WITH period1 AS (
		SELECT p1.PeopleId, SUM(c1.Amount) c1amt, 0 c2amt
		FROM dbo.People p1
		JOIN dbo.Contributions2(
			DATEADD(YEAR, -1, @fd) -- start date
			,DATEADD(YEAR, -1, @td) -- enddate
			,0, 0, 0, 1, null) c1 ON p1.PeopleId = c1.CreditGiverId 
		WHERE @fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c1.FundId)
		GROUP BY p1.PeopleId
	), 
	period2 AS (
		SELECT p2.PeopleId, 0 c1amt, SUM(c2.Amount) c2amt
		FROM dbo.People p2
		JOIN dbo.Contributions2(
			@fd ,@td
			,0, 0, 0, 1, null) c2 ON p2.PeopleId = c2.CreditGiverId 
		WHERE (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c2.FundId))

		GROUP BY p2.PeopleId
	), 
	bothperiods AS (
		SELECT PeopleId, c1amt, c2amt FROM period1
		UNION
		SELECT PeopleId, c1amt, c2amt FROM period2
	),
	grouped AS (
		SELECT 
			PeopleId 
			,TotalPeriod1 = ISNULL(SUM(c1amt), 0)
			,TotalPeriod2 = ISNULL(SUM(c2amt), 0)
		FROM bothperiods GROUP BY PeopleId
	), 
	getpercent AS (
		SELECT 
			PeopleId, TotalPeriod1, TotalPeriod2, 
			PctOfPrevious = (NULLIF(TotalPeriod2, 0) / NULLIF(TotalPeriod1, 0)) * 100
		FROM grouped
	) 
	SELECT 
		pc.PeopleId, 
		TotalPeriod1, 
		TotalPeriod2, 
		PctChange = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 100000.0
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN -100000.0
			ELSE PctOfPrevious - 100.0
		END,
		Change = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 'Started'
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN 'Stopped'
			WHEN PctOfPrevious > 100 THEN FORMAT(-(TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Increase'
			WHEN PctOfPrevious < 100 THEN FORMAT((TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Decrease'
			ELSE 'No Change'
		END
	FROM getpercent pc
	JOIN dbo.People p ON p.PeopleId = pc.PeopleId
	WHERE (@tagid IS NULL OR EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = pc.PeopleId))
)
GO

ALTER FUNCTION [dbo].[GivingChangeQuartersFund] (@fd datetime, @td datetime, @taxnontax varchar(50), @fundids VARCHAR(MAX), @tagid INT)
RETURNS TABLE 
AS
RETURN 
(
WITH period1 AS (
		SELECT p1.PeopleId, SUM(c1.Amount) c1amt, 0 c2amt
		FROM dbo.People p1
		JOIN dbo.Contributions2(
			dateadd(year, -1, @fd) -- start date
			,dateadd(year, -1, @td) -- enddate
			,0, 0, (case @taxnontax when 'TaxDed' then 0 when 'Both' then null else 1 end), 1, null) c1 on p1.PeopleId = c1.CreditGiverId 
		where @fundids is null or exists(select null from dbo.SplitInts(@fundids) where Value = c1.FundId)
		group by p1.PeopleId
	), 
	period2 as (
		select p2.PeopleId, 0 c1amt, sum(c2.Amount) c2amt
		from dbo.People p2
		join dbo.Contributions2(
			@fd ,@td
			,0, 0, (case @taxnontax when 'TaxDed' then 0 when 'Both' then null else 1 end), 1, null) c2 ON p2.PeopleId = c2.CreditGiverId 
		WHERE (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c2.FundId))

		GROUP BY p2.PeopleId
	), 
	bothperiods AS (
		SELECT PeopleId, c1amt, c2amt FROM period1
		UNION
		SELECT PeopleId, c1amt, c2amt FROM period2
	),
	grouped AS (
		SELECT 
			PeopleId 
			,TotalPeriod1 = ISNULL(SUM(c1amt), 0)
			,TotalPeriod2 = ISNULL(SUM(c2amt), 0)
		FROM bothperiods GROUP BY PeopleId
	), 
	getpercent AS (
		SELECT 
			PeopleId, TotalPeriod1, TotalPeriod2, 
			PctOfPrevious = (NULLIF(TotalPeriod2, 0) / NULLIF(TotalPeriod1, 0)) * 100
		FROM grouped
	) 
	SELECT 
		pc.PeopleId, 
		TotalPeriod1, 
		TotalPeriod2, 
		PctChange = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 100000.0
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN -100000.0
			ELSE PctOfPrevious - 100.0
		END,
		Change = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 'Started'
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN 'Stopped'
			WHEN PctOfPrevious > 100 THEN FORMAT(-(TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Increase'
			WHEN PctOfPrevious < 100 THEN FORMAT((TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Decrease'
			ELSE 'No Change'
		END
	FROM getpercent pc
	JOIN dbo.People p ON p.PeopleId = pc.PeopleId
	WHERE (@tagid IS NULL OR EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = pc.PeopleId))
)
GO

ALTER FUNCTION [dbo].[GivingChangeQuartersFund2] (@fd1 datetime, @td1 datetime, @fd2 datetime, @td2 datetime, @taxnontax varchar(50), @fundids VARCHAR(MAX), @tagid INT)
RETURNS TABLE 
AS
RETURN 
(
WITH period1 AS (
		SELECT p1.PeopleId, SUM(c1.Amount) c1amt, 0 c2amt
		FROM dbo.People p1
		JOIN dbo.Contributions2(
			@fd1 -- start date
			,@td1 -- enddate
			,0, 0, (case @taxnontax when 'TaxDed' then 0 when 'Both' then null else 1 end), 1, null) c1 on p1.PeopleId = c1.CreditGiverId 
		where @fundids is null or exists(select null from dbo.SplitInts(@fundids) where Value = c1.FundId)
		group by p1.PeopleId
	), 
	period2 as (
		select p2.PeopleId, 0 c1amt, sum(c2.Amount) c2amt
		from dbo.People p2
		join dbo.Contributions2(
			@fd2 ,@td2
			,0, 0, (case @taxnontax when 'TaxDed' then 0 when 'Both' then null else 1 end), 1, null) c2 ON p2.PeopleId = c2.CreditGiverId 
		WHERE (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c2.FundId))

		GROUP BY p2.PeopleId
	), 
	bothperiods AS (
		SELECT PeopleId, c1amt, c2amt FROM period1
		UNION
		SELECT PeopleId, c1amt, c2amt FROM period2
	),
	grouped AS (
		SELECT 
			PeopleId 
			,TotalPeriod1 = ISNULL(SUM(c1amt), 0)
			,TotalPeriod2 = ISNULL(SUM(c2amt), 0)
		FROM bothperiods GROUP BY PeopleId
	), 
	getpercent AS (
		SELECT 
			PeopleId, TotalPeriod1, TotalPeriod2, 
			PctOfPrevious = (NULLIF(TotalPeriod2, 0) / NULLIF(TotalPeriod1, 0)) * 100
		FROM grouped
	) 
	SELECT 
		pc.PeopleId, 
		TotalPeriod1, 
		TotalPeriod2, 
		PctChange = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 100000.0
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN -100000.0
			ELSE PctOfPrevious - 100.0
		END,
		Change = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 'Started'
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN 'Stopped'
			WHEN PctOfPrevious > 100 THEN FORMAT(-(TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Increase'
			WHEN PctOfPrevious < 100 THEN FORMAT((TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Decrease'
			ELSE 'No Change'
		END
	FROM getpercent pc
	JOIN dbo.People p ON p.PeopleId = pc.PeopleId
	WHERE (@tagid IS NULL OR EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = pc.PeopleId))
)
GO

ALTER FUNCTION [dbo].[GivingCurrentPercentOfFormer]
(
	@dt1 DATETIME, 
	@dt2 DATETIME,
	@comp nvarchar(2),
	@pct FLOAT
)
RETURNS TABLE 
AS
RETURN 
(
SELECT pid, c1amt, c2amt, pct
FROM (
	SELECT pid, c1amt, c2amt, NULLIF(c2amt, 0) * 100 / NULLIF(c1amt, 0) pct
	FROM (	
		SELECT pid, SUM(c1amt) c1amt, SUM(c2amt) c2amt
		FROM (	
			SELECT p1.PeopleId pid, SUM(c1.Amount) c1amt, 0 c2amt
			FROM dbo.People p1
			JOIN dbo.Contributions2(@dt1, @dt2, 0, 0, 0, 0, null) c1 ON p1.PeopleId = c1.CreditGiverId
			GROUP BY p1.PeopleId
			UNION
			SELECT p2.PeopleId pid, 0 c1amt, SUM(c2.Amount) c2amt
			FROM dbo.People p2
			JOIN dbo.Contributions2(@dt2, GETDATE(), 0, 0, 0, 0, null) c2 ON p2.PeopleId = c2.CreditGiverId
			GROUP BY p2.PeopleId
		) t1
		GROUP BY t1.pid
	) t2
) t3
WHERE (@comp <> '<=' OR t3.pct <= @pct)
AND (@comp <> '>' OR t3.pct > @pct)
)
GO

ALTER PROC [dbo].[LedgerIncomeExport]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded INT,
	@includeUnclosed BIT,
	@includeBundleType BIT
)
AS
BEGIN
	DECLARE @T TABLE (
			[COUNT] int,
			Amount decimal(12,2),
			FundId int,
			FundName nvarchar(512),
			[ONLINE] BIT,
			BundleType nvarchar(512),
			FundAccountCode int,
			FundIncomeDept nvarchar(512),
			FundIncomeAccount nvarchar(512),
			FundCashAccount nvarchar(512));


	WITH totals AS (
		SELECT 
			COUNT(*) AS [Count], 
			SUM(Amount) AS Amount, 
			c2.FundId, 
			FundName,
			BundleHeaderTypeId,
			CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END AS [OnLine]
		FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed, null) c2
		JOIN dbo.BundleHeader h ON c2.BundleHeaderId = h.BundleHeaderId
		GROUP BY c2.FundId, FundName, 
			CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END,
			BundleHeaderTypeId
	)
	INSERT INTO @T (
			[COUNT],
			Amount,
			FundId,
			FundName,
			[ONLINE],
			BundleType,
			FundAccountCode,
			FundIncomeDept,
			FundIncomeAccount,
			FundCashAccount)
		SELECT 
			tt.[COUNT] ,
			tt.Amount ,
			tt.FundId ,
			tt.FundName ,
			tt.[ONLINE] ,
			bht.[Description] BundleType,
			f.FundAccountCode ,
			f.FundIncomeDept ,
			f.FundIncomeAccount ,
			f.FundCashAccount 
		FROM totals tt
		JOIN lookup.BundleHeaderTypes bht ON tt.BundleHeaderTypeId = bht.Id
		JOIN dbo.ContributionFund f ON f.FundId = tt.FundId
		ORDER BY f.FundId, bht.Description;

	IF @includeBundleType = 1
		SELECT * FROM @T
	ELSE
		SELECT [COUNT], Amount, FundId, FundName, [ONLINE], FundAccountCode, FundIncomeDept, FundIncomeAccount, FundCashAccount FROM @T
	END
GO

ALTER FUNCTION [dbo].[PledgeBalances] ( @fundid INT )
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		c.CreditGiverId
		,SpouseId = c.CreditGiverId2
		,SUM(PledgeAmount) PledgeAmt
		,SUM(ISNULL(Amount, 0)) GivenAmt
		,CASE WHEN SUM(ISNULL(c.PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
	FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1, null) c
	WHERE c.FundId = @fundid
	GROUP BY c.CreditGiverId, c.CreditGiverId2
)
GO

ALTER FUNCTION [dbo].[PledgeFulfillment] ( @fundid INT )
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		p.PreferredName [First]
		,p.LastName [Last]
		,sp.PreferredName Spouse
		,ms.Description [MemberStatus]
		,(SELECT MIN(Date) FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1, null) WHERE FundId = @fundid AND CreditGiverId = c.CreditGiverId AND PledgeAmount > 0) PledgeDate
		,MAX(Date) LastDate
		,SUM(PledgeAmount) PledgeAmt
		,SUM(Amount) TotalGiven
		,CASE WHEN SUM(ISNULL(c.PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
		,p.PrimaryAddress [Address]
		,p.PrimaryCity [City]
		,p.PrimaryState [State]
		,p.PrimaryZip [Zip]
		,c.CreditGiverId
		,c.SpouseId
		,c.FamilyId
	FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1, null) c
	JOIN dbo.People p ON c.CreditGiverId = p.PeopleId
	JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	LEFT OUTER JOIN dbo.People sp ON p.SpouseId = sp.PeopleId
	WHERE c.FundId = @fundid
	GROUP BY c.CreditGiverId, c.SpouseId, p.PreferredName, p.LastName, sp.PreferredName, p.PrimaryAddress, p.PrimaryCity, p.PrimaryState, p.PrimaryZip, ms.Description, c.FamilyId
)
GO

ALTER PROC [dbo].[PledgeFulfillment2] ( @fundid1 INT, @fundid2 INT )
AS
BEGIN
	SELECT 
			CreditGiverId 
			, c.SpouseId
			, c.FamilyId
			, FundId
			, Date
			, PledgeAmount
			, Amount
			INTO #t
		FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1, null) c 
		WHERE c.FundId IN (@fundid1, @fundid2)

	SELECT tt.firstname
		,tt.LastName
		,tt.spouse
		,tt.memberstatus

		,c1.PledgeDate PledgeDate1
		,c2.PledgeDate PledgeDate2

		,c1.LastDate LastDate1
		,c2.LastDate LastDate2

		,c1.PledgeAmt PledgeAmt1
		,c2.PledgeAmt PledgeAmt2

		,c1.TotalGiven TotalGiven1
		,c2.TotalGiven TotalGiven2

		,c1.Balance Balance1
		,c2.Balance Balance2

		,tt.CreditGiverId
		,tt.SpouseId
		,tt.FamilyId
	FROM (
		SELECT 
			p.PreferredName firstname
			,p.LastName
			,sp.PreferredName spouse
			,ms.Description memberstatus
			,c.CreditGiverId
			,c.SpouseId
			,c.FamilyId
		FROM #t c
		JOIN dbo.People p ON c.CreditGiverId = p.PeopleId
		JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
		LEFT OUTER JOIN dbo.People sp ON p.SpouseId = sp.PeopleId
		WHERE c.FundId IN (@fundid1, @fundid2)
		GROUP BY p.PeopleId, c.CreditGiverId, c.SpouseId, p.PreferredName, p.LastName, sp.PreferredName, ms.Description, c.FamilyId
	) tt
	LEFT JOIN (
		SELECT CreditGiverId
		,(SELECT MIN(Date) FROM #t 
			WHERE PledgeAmount > 0 
			AND FundId = @fundid1 
			AND CreditGiverId = ct.CreditGiverId
		) PledgeDate
		,MAX(Date) LastDate
		,SUM(PledgeAmount) PledgeAmt
		,SUM(Amount) TotalGiven
		,CASE WHEN SUM(ISNULL(PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
		FROM #t ct
		WHERE FundId = @fundid1
		GROUP BY CreditGiverId
	) c1 ON c1.CreditGiverId = tt.CreditGiverId
	LEFT JOIN (
		SELECT CreditGiverId
		,(SELECT MIN(Date) FROM #t 
			WHERE PledgeAmount > 0 
			AND FundId = @fundid2 
			AND CreditGiverId = ct.CreditGiverId
		) PledgeDate
		,MAX(Date) LastDate
		,SUM(PledgeAmount) PledgeAmt
		,SUM(Amount) TotalGiven
		,CASE WHEN SUM(ISNULL(PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
		FROM #t ct
		WHERE FundId = @fundid2
		GROUP BY CreditGiverId
	) c2 ON c2.CreditGiverId = tt.CreditGiverId

	DROP TABLE #t

END
GO

ALTER FUNCTION [dbo].[PledgeReport]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT tt.FundId, tt.FundName, SUM(Plg) AS Plg, 
		SUM(CASE WHEN tt.Plg > 0 THEN tt.Amt END) AS ToPledge,
		SUM(CASE WHEN tt.Plg = 0 THEN tt.Amt END) AS NotToPledge,
		SUM(tt.Amt) AS ToFund
	FROM
	(
	SELECT SUM(ISNULL(Amount,0)) Amt, SUM(ISNULL(PledgeAmount, 0)) Plg, f.FundId, f.FundName
	FROM dbo.ContributionFund f
	LEFT JOIN Contributions2(@fd, @td, @campusid, NULL, NULL, 1, null) c ON c.FundId = f.FundId
	GROUP BY CreditGiverId, SpouseId, f.FundId, f.FundName, f.FundPledgeFlag
	HAVING  f.FundPledgeFlag = 1
	) tt
	GROUP BY FundId, tt.FundName
)
GO

ALTER FUNCTION [dbo].[Pledges0]
(
	@fd DATETIME, 
	@td DATETIME,
	@fundid INT,
	@campusid INT
)
RETURNS 
@t TABLE ( PeopleId INT NOT NULL )
AS
BEGIN
	DECLARE @cc TABLE (PeopleId INT)

	INSERT INTO @cc
	SELECT CreditGiverId
	FROM Contributions2(@fd, @td, @campusid, 1, NULL, 1, null)
    WHERE (ISNULL(@fundid, 0) = 0 OR FundId = @fundid)
    AND PledgeAmount > 0
	GROUP BY CreditGiverId

	UNION

	SELECT CreditGiverId2
	FROM Contributions2(@fd, @td, @campusid, 1, NULL, 1, null)
    WHERE (ISNULL(@fundid,0) = 0 OR FundId = @fundid)
    AND PledgeAmount > 0
	AND CreditGiverId2 IS NOT NULL
	GROUP BY CreditGiverId2

	INSERT @t ( PeopleId )
	SELECT p.PeopleId 
	FROM dbo.People p
	LEFT JOIN @cc c ON c.PeopleId = p.PeopleId
	WHERE c.PeopleId IS NULL
	
	RETURN 
END
GO

ALTER FUNCTION [dbo].[RecentGiver](@days INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
		SELECT *
		FROM Contributions2(
				DATEADD(DAY, -@days, GETDATE()),
				GETDATE(),
				0, 0, 0, 1, null)
		WHERE Amount > 0
	), BOTH AS (
		SELECT contributions.CreditGiverId PeopleId
		FROM contributions

		UNION

		SELECT contributions.SpouseId 
		FROM contributions
	)
	SELECT DISTINCT PeopleId FROM BOTH
	WHERE PeopleId IS NOT NULL
)
GO

ALTER FUNCTION [dbo].[RecentGiverFund](@days INT, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
		SELECT *
		FROM Contributions2(
				DATEADD(DAY, -@days, GETDATE()),
				GETDATE(),
				0, 0, 0, 1, null)
		WHERE Amount > 0
		AND FundId = @fundid
	), BOTH AS (
		SELECT contributions.CreditGiverId PeopleId
		FROM contributions

		UNION

		SELECT contributions.SpouseId 
		FROM contributions
	)
	SELECT DISTINCT PeopleId FROM BOTH
	WHERE PeopleId IS NOT NULL
)
GO

ALTER function [dbo].[RecentGiverFunds](@days int, @funds varchar(max))
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
		SELECT *
		FROM Contributions2(
				DATEADD(DAY, -@days, GETDATE()),
				GETDATE(),
				0, 0, 0, 1, null)
		WHERE Amount > 0
		AND FundId in (select value from dbo.SplitInts(@funds))
	), BOTH AS (
		SELECT contributions.CreditGiverId PeopleId
		FROM contributions

		UNION

		SELECT contributions.SpouseId 
		FROM contributions
	)
	SELECT DISTINCT PeopleId FROM BOTH
	WHERE PeopleId IS NOT NULL
)
GO