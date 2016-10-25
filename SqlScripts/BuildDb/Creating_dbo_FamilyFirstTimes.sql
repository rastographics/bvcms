CREATE VIEW [dbo].[FamilyFirstTimes]
AS
	WITH 
		firsttimes AS (
			SELECT 
				a.PeopleId,
				FirstDate = MIN(a.MeetingDate)
			FROM dbo.Attend a
			WHERE a.SeqNo = 1
			GROUP BY a.PeopleId, a.SeqNo
		), 
		fammembers AS (
			SELECT 
				p.PeopleId,
				p.FamilyId,
				NAME,
				ft.FirstDate,
				p.CreatedDate
			FROM dbo.People p
			LEFT JOIN firsttimes ft ON ft.PeopleId = p.PeopleId
		)
	SELECT
		f.FamilyId,
		f.HeadOfHouseholdId,
		FirstDate = MIN(p.FirstDate),
		CreatedDate = MIN(p.CreatedDate)
	FROM fammembers p
	JOIN dbo.Families f ON f.FamilyId = p.FamilyId
	GROUP BY f.FamilyId, f.HeadOfHouseholdId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
