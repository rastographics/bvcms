CREATE VIEW [dbo].[MeetingConflicts]
AS
		SELECT DISTINCT
			t1.PeopleId, 
			OrgId1 = (SELECT CASE WHEN t1.OrganizationId > t2.OrganizationId THEN t2.OrganizationId ELSE t1.OrganizationId END),
			OrgId2 = (SELECT CASE WHEN t1.OrganizationId < t2.OrganizationId THEN t2.OrganizationId ELSE t1.OrganizationId END),
			t1.MeetingDate
		FROM
		(
			SELECT a.PeopleId, a.MeetingDate, a.OrganizationId
			FROM dbo.Attend a
			WHERE Commitment IN (1,2,4)
			AND a.MeetingDate >= GETDATE()
		) t1
		JOIN
		(
			SELECT a.PeopleId, a.MeetingDate, a.OrganizationId
			FROM dbo.Attend a
			WHERE Commitment IN (1,2,4)
			AND a.MeetingDate >= GETDATE()
		) t2 ON t2.MeetingDate = t1.MeetingDate AND t2.PeopleId = t1.PeopleId AND t1.OrganizationId <> t2.OrganizationId
		GROUP BY t1.OrganizationId, t2.OrganizationId, t1.PeopleId, t1.MeetingDate
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
