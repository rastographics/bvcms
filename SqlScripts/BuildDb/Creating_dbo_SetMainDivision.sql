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
IF @@ERROR <> 0 SET NOEXEC ON
GO
