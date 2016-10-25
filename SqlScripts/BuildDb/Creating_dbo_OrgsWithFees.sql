
CREATE VIEW [dbo].[OrgsWithFees]
AS
(
	SELECT 
		OrganizationId
	FROM dbo.Organizations
WHERE dbo.OrgFee(OrganizationId) > 0
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
