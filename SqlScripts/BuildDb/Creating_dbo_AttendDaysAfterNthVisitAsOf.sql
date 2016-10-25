-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[AttendDaysAfterNthVisitAsOf] (	 @progid INT, @divid INT, @org INT, @d1 DATETIME, @d2 DATETIME, @n INT, @days INT )
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		a.PeopleId
		FROM dbo.Attend a
			WHERE AttendanceFlag = 1 AND SeqNo = @n
			AND a.MeetingDate >= @d1
			AND a.MeetingDate <= @d2
			AND (EXISTS(
				SELECT NULL FROM dbo.Attend a2
				WHERE a2.AttendanceFlag = 1
				AND a2.PeopleId = a.PeopleId
				AND a2.Seqno > @n
				AND a2.MeetingDate <= DATEADD(dd, @days, a.MeetingDate)
				AND (ISNULL(@org, 0) = 0 OR a2.OrganizationId = @org)
				AND (ISNULL(@divid, 0) = 0 
						OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = a2.OrganizationId AND DivId = @divid))
				AND (ISNULL(@progid, 0) = 0 
						OR EXISTS(SELECT NULL FROM dbo.DivOrg dd WHERE dd.OrgId = a2.OrganizationId
							AND EXISTS(SELECT NULL FROM dbo.ProgDiv pp WHERE pp.DivId = dd.DivId AND pp.ProgId = @progid))))
			)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
