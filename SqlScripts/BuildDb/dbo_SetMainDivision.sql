-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetMainDivision](@orgid INT, @divid INT)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE dbo.DivOrg
	SET id = NULL
	WHERE OrgId = @orgid
	
	UPDATE dbo.DivOrg
	SET id = 1
	WHERE OrgId = @orgid AND DivId = @divid
	
	UPDATE dbo.Organizations
	SET DivisionId = @divid
	WHERE OrganizationId = @orgid
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
