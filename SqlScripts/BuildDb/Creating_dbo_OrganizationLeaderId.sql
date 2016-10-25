CREATE FUNCTION [dbo].[OrganizationLeaderId](@orgid int)
RETURNS int
AS
BEGIN
	DECLARE @id int, @orgstatus int
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
                      om.MemberTypeId = o.LeaderMemberTypeId AND
                      (om.Pending IS NULL OR om.Pending = 0)
	where om.OrganizationId = @orgid
	ORDER BY om.EnrollmentDate
	RETURN @id

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
