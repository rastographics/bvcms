-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[RecentAttendMemberType]( 
	@progid INT,
	@divid INT,
	@org INT,
	@days INT,
	@idstring VARCHAR(500))
RETURNS 
@t TABLE 
(
	PeopleId INT
)
AS
BEGIN
	DECLARE @ids TABLE(id INT)
	INSERT @ids SELECT Value FROM dbo.SplitInts(@idstring)

	INSERT @t
	SELECT 
		p.PeopleId 
		FROM dbo.People p
		WHERE EXISTS(
			SELECT NULL
			FROM dbo.Attend a
			JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE 1 = 1
			AND a.MemberTypeId IN (SELECT id FROM @ids)
			AND  AttendanceFlag = 1
			AND a.PeopleId = p.PeopleId
			AND a.MeetingDate > DATEADD(dd, -@days, GETDATE())
			AND (ISNULL(@org, 0) = 0 OR m.OrganizationId = @org)
			AND (ISNULL(@divid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = m.OrganizationId AND DivId = @divid))
			AND (ISNULL(@progid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg dd WHERE dd.OrgId = m.OrganizationId
						AND EXISTS(SELECT NULL FROM dbo.ProgDiv pp WHERE pp.DivId = dd.DivId AND pp.ProgId = @progid)))
		)
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
