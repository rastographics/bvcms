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
)--OrgName,OrgStatus,OnlineReg,OrgType,Schedule,Campus,Program,Division,Organization
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT  PeopleId
	FROM Organizations o 
	JOIN dbo.OrganizationMembers om ON om.OrganizationId = o.OrganizationId
	WHERE (@name IS NULL OR o.OrganizationName LIKE '%' + @name + '%')
	
	AND (ISNULL(@prog, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg di
					JOIN dbo.ProgDiv pp ON pp.DivId = di.DivId
					WHERE di.OrgId = o.OrganizationId AND pp.ProgId = @prog))
					
	AND (ISNULL(@div, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg dd
					WHERE dd.OrgId = o.OrganizationId AND dd.DivId = @div))
					
	AND (ISNULL(@type, 0) = 0 
			OR o.OrganizationTypeId = @type
			OR (@type = -1 AND o.OrganizationTypeId IS NULL)
			OR (@type = -2 AND ISNULL(o.IsBibleFellowshipOrg, 0) = 0)
			OR (@type = -3 AND o.IsBibleFellowshipOrg = 1)
			OR (@type = -4 AND o.SuspendCheckin = 1)
			OR (@type = -5 AND EXISTS(SELECT NULL FROM dbo.Organizations WHERE ParentOrgId = o.OrganizationId))
			OR (@type = -6 AND o.ParentOrgId > 0))

	AND (ISNULL(@campus, 0) = 0 OR o.CampusId = @campus)
	
	AND (ISNULL(@sched, 0) = 0
		OR (@sched = -1 AND NOT EXISTS(SELECT NULL FROM dbo.OrgSchedule WHERE OrganizationId = o.OrganizationId))
		OR EXISTS(SELECT NULL FROM dbo.OrgSchedule WHERE OrganizationId = o.OrganizationId AND ScheduleId = @sched))
					
	AND (ISNULL(@status, 0) = 0 OR o.OrganizationStatusId = @status)
	
	AND ((@onlinereg = 0 AND ISNULL(o.RegistrationTypeId, 0) = 0) 
		  OR (@onlinereg = 99 AND ISNULL(o.RegistrationTypeId, 0) > 0) 
		  OR (@onlinereg > 0 AND ISNULL(o.RegistrationTypeId, 0) = @onlinereg) 
		  OR @onlinereg = -1
		  OR @onlinereg IS NULL
		)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
