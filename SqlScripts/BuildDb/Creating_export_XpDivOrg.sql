CREATE VIEW [export].[XpDivOrg] AS 
SELECT 
	DivId ,
    OrgId 
FROM dbo.DivOrg
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
