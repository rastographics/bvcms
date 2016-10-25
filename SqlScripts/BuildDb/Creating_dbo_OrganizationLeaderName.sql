CREATE FUNCTION [dbo].[OrganizationLeaderName](@orgid int)
RETURNS nvarchar(100)
AS
BEGIN
	DECLARE @id int, @orgstatus int, @name nvarchar(100)
	select @orgstatus = OrganizationStatusId 
	from dbo.Organizations
	where OrganizationId = @orgid
	--if (@orgstatus = 40)
	--SELECT top 1 @id = PeopleId from
 --                     dbo.EnrollmentTransaction et INNER JOIN
 --                     dbo.Organizations o ON 
 --                     et.OrganizationId = o.OrganizationId AND 
 --                     et.MemberTypeId = o.LeaderMemberTypeId
	--		where et.OrganizationId = @orgid
 --                     order by et.TransactionDate desc
	--else
	SELECT top 1 @id = PeopleId from
                      dbo.OrganizationMembers om INNER JOIN
                      dbo.Organizations o ON 
                      om.OrganizationId = o.OrganizationId AND 
                      om.MemberTypeId = o.LeaderMemberTypeId
	where om.OrganizationId = @orgid
	ORDER BY om.EnrollmentDate

	SELECT @name = (case when [Nickname]<>'' then [nickname] else [FirstName] end+' ')+[LastName] from dbo.People where PeopleId = @id
	RETURN @name
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
