-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[AttendanceTypeAsOf]
(	
	@from DATETIME,
	@to DATETIME,
	@progid INT,
	@divid INT,
	@orgid INT,
	@orgtype INT,
	@ids nvarchar(4000)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT p.PeopleId FROM dbo.People p
	WHERE EXISTS (
		SELECT NULL FROM dbo.Attend a
		JOIN dbo.Meetings m ON m.MeetingId = a.MeetingId
		JOIN dbo.Organizations o ON o.OrganizationId = m.OrganizationId
		WHERE a.PeopleId = p.PeopleId
		AND a.AttendanceTypeId IN (SELECT id FROM CsvTable(@ids))
		AND a.AttendanceFlag = 1
		AND a.MeetingDate >= @from
		AND a.MeetingDate < @to
        AND (o.OrganizationTypeId = @orgtype OR @orgtype = 0)
		AND (a.OrganizationId = @orgid OR @orgid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d1
				WHERE d1.OrgId = a.OrganizationId
				AND d1.DivId = @divid) OR @divid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d2
				WHERE d2.OrgId = a.OrganizationId
				AND EXISTS(SELECT NULL FROM ProgDiv pd
						WHERE d2.DivId = pd.DivId
						AND pd.ProgId = @progid)) OR @progid = 0)
		)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
