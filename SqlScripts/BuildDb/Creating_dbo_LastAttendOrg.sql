CREATE FUNCTION [dbo].[LastAttendOrg](@oid INT)
RETURNS TABLE
AS
RETURN
(
	SELECT a.PeopleId
	, a.MeetingDate LastAttended

	FROM dbo.Attend a
	JOIN dbo.People p ON p.PeopleId = a.PeopleId
	JOIN dbo.Organizations o ON o.OrganizationId = a.OrganizationId
	JOIN lookup.AttendType at ON at.Id = a.AttendanceTypeId
	WHERE a.OrganizationId = @oid
	AND a.AttendanceFlag = 1
	AND a.MeetingDate = (SELECT TOP 1 MeetingDate 
							FROM dbo.Attend
							WHERE AttendanceFlag = 1 
							AND OrganizationId = @oid 
							AND PeopleId = a.PeopleId
							ORDER BY MeetingDate DESC
						)
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
