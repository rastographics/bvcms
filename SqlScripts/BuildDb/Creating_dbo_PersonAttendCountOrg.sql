-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[PersonAttendCountOrg]
(@pid int, @oid int)
RETURNS int
AS
	BEGIN
	RETURN (SELECT COUNT(*)
	        FROM   dbo.Attend a INNER JOIN
	                   dbo.Meetings m ON a.MeetingId = m.MeetingId
	        WHERE (m.OrganizationId = @oid) AND (a.PeopleId = @pid)
              AND a.AttendanceFlag = 1)
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
