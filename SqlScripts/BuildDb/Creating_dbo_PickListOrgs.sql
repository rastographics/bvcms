


CREATE VIEW [dbo].[PickListOrgs]
AS

SELECT OrgId FROM Organizations o 
CROSS APPLY ( SELECT CAST(value AS INT) OrgId 
	FROM dbo.Split(o.OrgPickList, ',') ) T 
WHERE o.OrgPickList IS NOT NULL


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
