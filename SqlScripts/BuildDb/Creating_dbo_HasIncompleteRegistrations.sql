CREATE FUNCTION [dbo].[HasIncompleteRegistrations]
(	
	@prog INT
	,@div INT 
	,@org INT 
	,@begdt DATETIME
	,@enddt DATETIME
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT  r.PeopleId
	FROM dbo.RegistrationList r
	JOIN dbo.Organizations o ON o.OrganizationId = r.OrganizationId
	WHERE r.PeopleId IS NOT NULL
	AND r.Stamp >= @begdt
	AND r.Stamp <= @enddt
	
	AND (ISNULL(@prog, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg di
					JOIN dbo.ProgDiv pp ON pp.DivId = di.DivId
					WHERE di.OrgId = o.OrganizationId AND pp.ProgId = @prog))
					
	AND (ISNULL(@div, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg dd
					WHERE dd.OrgId = o.OrganizationId AND dd.DivId = @div))

	AND (ISNULL(@org, 0) = 0 OR o.OrganizationId = @org)
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
