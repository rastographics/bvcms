CREATE FUNCTION [dbo].[RecentAttendType]( 
	@progid INT,
	@divid INT,
	@org INT,
	@days INT,
	@idstring VARCHAR(500))
RETURNS 
@t TABLE ( PeopleId INT )
AS
BEGIN
	DECLARE @ids TABLE(id INT)
	INSERT @ids SELECT Value FROM dbo.SplitInts(@idstring)
	DECLARE @cnt INT, @firstid INT
	SELECT @cnt = COUNT(*) FROM @ids  
	SELECT TOP 1 @firstid = id FROM @ids;  

	INSERT @t
	SELECT 
		p.PeopleId 
		FROM dbo.People p
		WHERE EXISTS(
			SELECT NULL
			FROM dbo.Attend a
			JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE (AttendanceFlag = 1 
				OR (@cnt = 1 AND @firstid = 80)) -- offsite
			AND ISNULL(a.AttendanceTypeId, 0) IN (SELECT id FROM @ids)
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
