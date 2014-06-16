CREATE FUNCTION [dbo].[PeopleIdsFromOrgSearch]
(	
	 @name nvarchar(100)
	,@prog INT
	,@div INT 
	,@type INT 
	,@campus INT
	,@sched INT 
	,@status INT
	,@onlinereg INT
	,@mainfellowship BIT
	,@parentorg BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT  PeopleId
	FROM Organizations o 
	JOIN dbo.OrganizationMembers om ON om.OrganizationId = o.OrganizationId
	WHERE (@name IS NULL OR o.OrganizationName LIKE '%' + @name + '%')
	
	AND (ISNULL(@prog, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg dd
					JOIN dbo.Division di ON dd.DivId = di.Id
					JOIN dbo.ProgDiv pp ON di.Id = pp.DivId
					WHERE dd.OrgId = o.OrganizationId AND pp.ProgId = @prog))
					
	AND (ISNULL(@div, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg dd
					WHERE dd.OrgId = o.OrganizationId AND dd.DivId = @div))
					
	AND (ISNULL(@type, 0) = 0 OR o.OrganizationTypeId = @type)
	
	AND (ISNULL(@campus, 0) = 0 OR o.CampusId = @campus)
	
	AND (ISNULL(@sched, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.OrgSchedule os 
					WHERE os.OrganizationId = o.OrganizationId AND os.ScheduleId = @sched))
					
	AND (ISNULL(@status, 0) = 0 OR o.OrganizationStatusId = @status)
	
	AND ((@onlinereg = 0 AND ISNULL(o.RegistrationTypeId, 0) = 0) 
		  OR (@onlinereg = 99 AND ISNULL(o.RegistrationTypeId, 0) > 0) 
		  OR (@onlinereg > 0 AND ISNULL(o.RegistrationTypeId, 0) = @onlinereg) 
		  OR @onlinereg = -1
		  OR @onlinereg IS NULL
		)
	AND (ISNULL(@mainfellowship, 0) = 0 OR (@mainfellowship = 1 AND o.IsBibleFellowshipOrg = 1))
	
	AND (ISNULL(@parentorg, 0) = 0 
		OR (ISNULL(@parentorg, 0) = 1 AND EXISTS(SELECT NULL FROM dbo.Organizations co WHERE co.ParentOrgId = o.OrganizationId)))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
