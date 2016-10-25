-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[delDivOrg]
   ON  [dbo].[DivOrg]
   AFTER DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Organizations
	SET DivisionId = NULL
	FROM dbo.Organizations o
	JOIN DELETED i ON i.OrgId = o.OrganizationId
	WHERE o.DivisionId = i.DivId
	
	UPDATE dbo.Organizations
	SET DivisionId = (SELECT TOP 1 DivId FROM dbo.DivOrg WHERE OrgId = o.OrganizationId)
	FROM dbo.Organizations o
	WHERE o.OrganizationId IN (SELECT OrgId FROM DELETED)
	AND o.DivisionId IS NULL
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
