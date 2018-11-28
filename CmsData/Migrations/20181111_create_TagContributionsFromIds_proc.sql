DROP PROCEDURE IF EXISTS [dbo].[TagContributionsFromIds]
GO
CREATE proc [dbo].[TagContributionsFromIds] (@tagname varchar(50), @json varchar(max), @ids varchar(max))
as
begin
	declare @AddToTag int = (select json_value(@json, '$.AddToTag'))
	declare @Priority int = (select json_value(@json, '$.Priority'))

	if isnull(@AddToTag, 0) = 0
	begin
		delete dbo.ContributionTag where TagName = @tagname

		insert dbo.ContributionTag ( ContributionId , TagName, [Priority] )
		select [Value], @tagname, @priority
		from dbo.SplitInts(@ids)
	end
	else
	begin
		insert dbo.ContributionTag ( ContributionId , TagName, [Priority] )
		select [Value], @tagname, @Priority
		from dbo.SplitInts(@ids)
		where not exists(select null from dbo.ContributionTag ct where [Value] = ct.ContributionId and ct.TagName = @tagname)
	end

end
GO

