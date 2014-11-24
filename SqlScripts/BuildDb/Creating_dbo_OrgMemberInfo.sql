CREATE FUNCTION [dbo].[OrgMemberInfo] (@oid INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT om.PeopleId
		,LastMeetingId = a.MeetingId 
		,LastAttendedDt = a.MeetingDate 
		--,c.ContactDate 
		--,c.ContactId 
		--,TaskAboutDt = ta.CreatedOn
		--,TaskAboutId = ta.Id 
		--,TaskDelegatedDate = td.CreatedOn 
		--,TaskDelegatedId = td.Id
	FROM dbo.OrganizationMembers om
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	LEFT JOIN Attend a ON a.OrganizationId = @oid AND a.PeopleId = om.PeopleId
		 AND a.MeetingDate = (SELECT MAX(la.MeetingDate) FROM dbo.Attend la
						     WHERE la.AttendanceFlag = 1 
							 AND la.OrganizationId = @oid 
							 AND la.PeopleId = a.PeopleId)
	--LEFT JOIN dbo.Contact c ON c.ContactId = (SELECT MAX(lc.ContactId) 
	--											FROM dbo.Contact lc
	--											JOIN dbo.Contactees ee ON ee.ContactId = lc.ContactId 
	--											AND ee.PeopleId = om.PeopleId)
	--LEFT JOIN dbo.Task ta ON ta.WhoId = om.PeopleId AND ta.StatusId IN (10, 20) -- Active, Waiting
	--	 AND ta.Id = (SELECT MAX(lt.Id) FROM dbo.Task lt
	--						WHERE lt.WhoId = om.PeopleId AND lt.StatusId IN (10, 20))
	--LEFT JOIN dbo.Task td ON td.WhoId = om.PeopleId AND td.StatusId IN (10, 20) -- Active, Waiting
	--	 AND td.Id = (SELECT MAX(lt.Id) FROM dbo.Task lt
	--						WHERE lt.WhoId = om.PeopleId AND lt.StatusId IN (10, 20))
	WHERE om.OrganizationId = @oid 
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
