CREATE VIEW [export].[XpOrgMember] AS 
SELECT OrganizationId ,
       PeopleId ,
       MemberTypeId ,
       EnrollmentDate ,
       InactiveDate ,
       Pending 
FROM dbo.OrganizationMembers
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
