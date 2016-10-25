CREATE FUNCTION [dbo].[RecentAbsents2](@orgid INT, @divid INT, @days INT)
RETURNS 
@ret TABLE 
(
OrganizationId INT,
OrganizationName nvarchar(70),
LeaderName nvarchar(60),
consecutive INT,
PeopleId INT,
Name2 nvarchar(50),
HomePhone nvarchar(15),
CellPhone nvarchar(15),
EmailAddress nvarchar(50),
MeetingId INT,
ConsecutiveAbsentsThreshhold INT
)
AS
BEGIN
DECLARE @t TABLE ( Oid INT NOT NULL, Pid INT NOT NULL, consecutive INT,
PRIMARY KEY (Oid, Pid));

WITH LastAbsents AS
(
  SELECT OrganizationId, PeopleId, MAX(MeetingDate) ld
  FROM Attend
  WHERE EffAttendFlag = 0
  GROUP BY OrganizationId, PeopleId
)
INSERT INTO @t (Oid, Pid, consecutive) 
SELECT a.OrganizationId, a.PeopleId, count(*) As ConsecutiveAbsents
FROM dbo.Attend a
INNER JOIN LastAbsents la 
			ON la.OrganizationId = a.OrganizationId 
			AND la.PeopleId = a.PeopleId
			AND a.MeetingDate > la.ld
			AND a.MeetingDate < GETDATE()
		WHERE (a.OrganizationId = @orgid OR @orgid IS NULL)
		AND (@divid IS NULL 
			OR EXISTS(SELECT NULL 
						FROM dbo.DivOrg 
						WHERE OrgId = a.OrganizationId AND DivId = @divid))
group by a.OrganizationId, a.PeopleId
		
		
INSERT INTO @ret (
	OrganizationId,
	OrganizationName,
	LeaderName,
	consecutive,
	PeopleId,
	Name2,
	HomePhone,
	CellPhone,
	EmailAddress,
	ConsecutiveAbsentsThreshhold
)
	SELECT 
		 pp.OrganizationId
		,oo.OrganizationName
		,oo.LeaderName
		,consecutive
		,pp.PeopleId 
		,pp.Name2
		,pp.HomePhone
		,pp.CellPhone
		,pp.EmailAddress
		,ConsecutiveAbsentsThreshold
	FROM @t t
	JOIN
	(
		SELECT 
			m.PeopleId, 
			p.Name2, 
			p.EmailAddress, 
			p.HomePhone, 
			p.CellPhone, 
			m.OrganizationId, 
			m.MemberTypeId, 
			at.Id AttendTypeId
		FROM dbo.OrganizationMembers m
		JOIN lookup.MemberType mt ON m.MemberTypeId = mt.Id
		JOIN lookup.AttendType at ON at.Id = mt.AttendanceTypeId
		JOIN dbo.People p ON m.PeopleId = p.PeopleId
	) pp ON t.Pid = pp.PeopleId AND t.Oid = pp.OrganizationId
	JOIN
	(
		SELECT 
			o.OrganizationId, 
			o.OrganizationName, 
			o.LeaderName, 
			ISNULL(o.ConsecutiveAbsentsThreshold, 2) AS ConsecutiveAbsentsThreshold
		FROM dbo.Organizations o
		WHERE (o.OrganizationId = @orgid OR @orgid IS NULL)
		AND (@divid IS NULL 
			OR EXISTS(SELECT NULL 
						FROM dbo.DivOrg 
						WHERE OrgId = o.OrganizationId AND DivId = @divid))
	) oo ON pp.OrganizationId = oo.OrganizationId
	WHERE consecutive >= ISNULL(oo.ConsecutiveAbsentsThreshold, 2)
	AND pp.AttendTypeId NOT IN (70, 100) --inservice and homebound
	AND pp.MemberTypeId != 230 --inactive	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
