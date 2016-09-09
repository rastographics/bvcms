--default.v1   IMPORTANT! delete this line if you want to customize, otherwise your changes will be overwritten
SELECT
	p.PeopleId,
	p.LastName,
	p.FamilyId,
	Name = p.Name2,
	p.PreferredName,
	Suffix = p.SuffixCode,
	DOB = dbo.DOB(p.BirthMonth,p.BirthDay, p.BirthYear),
	[Address] = p.PrimaryAddress,
	Address2 = p.PrimaryAddress2,
	City = p.PrimaryCity,
	[State] = p.PrimaryState,
	Zip = p.PrimaryZip,
	Cell = p.CellPhone,
	Home = p.HomePhone,
	Email = p.EmailAddress,
	Email2 = p.EmailAddress2,
	PhonesOk = CONVERT(BIT, IIF(p.DoNotPublishPhones = 1, 0, 1)),
	pp.MediumId,
	pp.SmallId,
	pp.X,
	pp.Y,
	PicDate = pp.CreatedDate,
	p.GenderId
FROM dbo.People p
JOIN dbo.Families f ON f.FamilyId = p.FamilyId
JOIN dbo.People hh ON hh.PeopleId = f.HeadOfHouseholdId
LEFT JOIN dbo.Picture pp ON pp.PictureId = p.PictureId 
JOIN dbo.TagPerson tp ON tp.PeopleId = p.PeopleId
WHERE tp.Id = @p1