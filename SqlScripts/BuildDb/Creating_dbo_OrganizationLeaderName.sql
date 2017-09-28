CREATE FUNCTION [dbo].[OrganizationLeaderName](@orgid int)
RETURNS nvarchar(100)
AS
BEGIN
	DECLARE @id int, @orgstatus int, @name nvarchar(100)
	select @orgstatus = OrganizationStatusId 
	FROM dbo.Organizations
	WHERE OrganizationId = @orgid
	SELECT TOP 1 @id = PeopleId FROM
                      dbo.OrganizationMembers om INNER JOIN
                      dbo.Organizations o ON 
                      om.OrganizationId = o.OrganizationId AND 
                      om.MemberTypeId = o.LeaderMemberTypeId
	where om.OrganizationId = @orgid
	AND ISNULL(om.Pending, 0) = 0
	ORDER BY om.EnrollmentDate

	SELECT @name = (case when [Nickname]<>'' then [nickname] else [FirstName] end+' ')+[LastName] from dbo.People where PeopleId = @id
	RETURN @name
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
