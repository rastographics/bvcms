CREATE FUNCTION [dbo].[FamilyMembers](@pid INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
	     Id = p.PeopleId,
		 PortraitId = ISNULL(pp.SmallId, IIF(p.GenderId = 1, -7, IIF(p.GenderId = 2, -8, -6))),
		 PicDate = pp.CreatedDate,
		 PicXPos = pp.X,
		 PicYPos = pp.Y,
		 p.DeceasedDate,
	     p.Name,
	     p.Age,
		 p.GenderId,
		 Color = '',
		 p.PositionInFamilyId,
		 PositionInFamily = 
			IIF(p.PositionInFamilyId = 10, 
				IIF(f.HeadOfHouseholdId = p.PeopleId, 'Head', 
					IIF(f.HeadOfHouseholdSpouseId = p.PeopleId, 'Spouse', 'Head2')), 
				fp.Description), 
	     SpouseIndicator = IIF(p.PeopleId = m.SpouseId, '*', ''),
	     Email = p.EmailAddress,
	     isDeceased = CAST(IIF(p.DeceasedDate IS NULL, 0, 1) AS BIT),
	     MemberStatus = ms.Description,
	     Gender = g.Code
	FROM dbo.People p
	JOIN dbo.People m ON m.FamilyId = p.FamilyId
	JOIN dbo.Families f ON f.FamilyId = p.FamilyId
	LEFT JOIN dbo.Picture pp ON pp.PictureId = p.PictureId
	LEFT JOIN lookup.FamilyPosition fp ON fp.Id = p.PositionInFamilyId
	LEFT JOIN lookup.Gender g ON g.Id = p.GenderId
	LEFT JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
	WHERE m.PeopleId = @pid
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
