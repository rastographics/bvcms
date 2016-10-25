CREATE VIEW [dbo].[ProspectCounts] AS
SELECT OrganizationId, COUNT(*) prospectcount
FROM dbo.OrganizationMembers
WHERE MemberTypeId = 311
GROUP BY OrganizationId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
