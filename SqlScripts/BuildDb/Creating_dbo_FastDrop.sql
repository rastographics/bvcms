CREATE PROCEDURE [dbo].[FastDrop](@oid INT, @pid INT, @dropdate DATETIME, @orgname NVARCHAR(100))
AS
BEGIN
	SET NOCOUNT ON

	INSERT dbo.EnrollmentTransaction
	        ( CreatedBy ,
	          CreatedDate ,
	          TransactionDate ,
	          TransactionTypeId ,
	          OrganizationId ,
	          OrganizationName ,
	          PeopleId ,
	          MemberTypeId
	        )
	SELECT 0, GETDATE(), @dropdate, 5, @oid, @orgname, @pid, om.MemberTypeId
	FROM dbo.OrganizationMembers om
	WHERE om.PeopleId = @pid AND om.OrganizationId = @oid
            
	DELETE dbo.OrgMemMemTags 
	WHERE PeopleId = @pid AND OrgId = @oid

	DELETE dbo.OrganizationMembers 
	WHERE PeopleId = @pid AND OrganizationId = @oid
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
