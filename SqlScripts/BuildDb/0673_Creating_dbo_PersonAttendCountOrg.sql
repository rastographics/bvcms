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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
