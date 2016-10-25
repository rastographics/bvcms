CREATE FUNCTION [dbo].[ConsecutiveAbsents](@orgid INT, @divid INT, @days INT)
RETURNS TABLE
AS
RETURN
(
	WITH LastAbsents AS
	(
	  SELECT OrganizationId, PeopleId, MAX(MeetingDate) ld
	  FROM Attend
	  WHERE AttendanceFlag = 1
	  GROUP BY OrganizationId, PeopleId
	)
	SELECT a.OrganizationId, a.PeopleId, count(*) As consecutive,
			(SELECT MAX(MeetingDate) 
			 FROM dbo.Attend aa 
			 WHERE aa.PeopleId = a.PeopleId  
			 AND aa.OrganizationId = a.OrganizationId 
			 AND aa.AttendanceFlag = 1) 
		lastattend
	FROM dbo.Attend a
	JOIN LastAbsents la 
				ON la.OrganizationId = a.OrganizationId 
				AND la.PeopleId = a.PeopleId
				AND a.MeetingDate > la.ld
			where (a.OrganizationId = @orgid OR @orgid IS NULL)
			AND (@divid IS NULL 
				OR EXISTS(SELECT NULL 
							FROM dbo.DivOrg 
							WHERE OrgId = a.OrganizationId AND DivId = @divid))
	group by a.OrganizationId, a.PeopleId
)
/*
	SELECT 
		tt.OrganizationId, 
		--o.OrganizationName, 
		--o.LeaderName, 
		consecutive,
		tt.PeopleId,
		--p.Name2,
		--p.HomePhone,
		--p.CellPhone,
		--p.EmailAddress,
			(SELECT MAX(MeetingDate) 
			 FROM dbo.Attend aa 
			 WHERE aa.PeopleId = tt.PeopleId  
			 AND aa.OrganizationId = tt.OrganizationId 
			 AND aa.AttendanceFlag = 1) 
		lastattend
		--lastmeeting,
		--m.MeetingId,
		--ConsecutiveAbsentsThreshold
	FROM
	(
	SELECT a.OrganizationId, a.PeopleId, count(*) As consecutive
	FROM dbo.Attend a
	JOIN LastAbsents la 
				ON la.OrganizationId = a.OrganizationId 
				AND la.PeopleId = a.PeopleId
				AND a.MeetingDate > la.ld
	group by a.OrganizationId, a.PeopleId
	) tt
	JOIN dbo.Organizations o ON tt.OrganizationId = o.OrganizationId
	JOIN dbo.Meetings m ON tt.OrganizationId = m.OrganizationId 
		AND m.MeetingDate = ( SELECT MAX(a.MeetingDate) FROM dbo.Attend a
							  JOIN dbo.Meetings mm ON mm.MeetingId = a.MeetingId
							  WHERE mm.OrganizationId = tt.OrganizationId
							  AND a.AttendanceFlag = 1 
							  AND mm.MeetingDate > DATEADD(d, -@days, GETDATE()) 
							  AND mm.MeetingDate < GETDATE()
							)
	JOIN OrganizationMembers om ON om.PeopleId = tt.PeopleId AND om.OrganizationId = o.OrganizationId
	JOIN dbo.People p ON tt.PeopleId = p.PeopleId
	JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
	JOIN lookup.AttendType at ON at.Id = mt.AttendanceTypeId
	WHERE consecutive > ISNULL(ConsecutiveAbsentsThreshold, 2)
	AND m.MeetingDate IS NOT NULL
	AND at.Id NOT IN (70, 100) --inservice and homebound
	AND om.MemberTypeId != 230 --inactive
			AND (tt.OrganizationId = @orgid OR @orgid IS NULL)
			AND (@divid IS NULL 
				OR EXISTS(SELECT NULL 
							FROM dbo.DivOrg 
							WHERE OrgId = tt.OrganizationId AND DivId = @divid))
*/	
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
