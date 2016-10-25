-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[MembersWhoAttendedOrgs]
(
	@orgs VARCHAR(MAX),
	@firstdate datetime
)
RETURNS 
@tt TABLE 
(
	PeopleId INT NOT NULL,
	Name2 VARCHAR(90),
	Age INT,
	BibleFellowshipClassId INT,
	OrganizationName VARCHAR(100),
	LeaderName VARCHAR(90)
)
AS
BEGIN
	DECLARE @t TABLE (id INT)
	INSERT INTO @t SELECT i.Value FROM dbo.SplitInts(@orgs) i

	INSERT INTO @tt
		SELECT DISTINCT a.PeopleId
			, p.Name2
			, p.Age
			, p.BibleFellowshipClassId
			, o.OrganizationName
			, o.LeaderName
		FROM dbo.Attend a
		JOIN dbo.People p ON p.PeopleId = a.PeopleId
		LEFT JOIN dbo.Organizations o ON o.OrganizationId = p.BibleFellowshipClassId
		WHERE a.OrganizationId IN (SELECT Id FROM @t)
		AND a.MeetingDate > @firstdate
		-- exclude RecentVisitor, NewVisitor, Prospect, Group
		AND a.AttendanceTypeId NOT IN (50, 60, 190, 90)
	RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
