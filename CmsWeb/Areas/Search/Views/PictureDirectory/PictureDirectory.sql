SELECT
	p.PeopleId,
	p.LastName,
	p.FamilyId,
	p.Name,
	p.PreferredName,
	p.SuffixCode,
	DOB = dbo.DOB(p.BirthMonth,p.BirthDay, p.BirthYear),
	[Address] = p.PrimaryAddress,
	Address2 = p.PrimaryAddress2,
	City = p.PrimaryCity,
	[State] = p.PrimaryState,
	Zip = p.PrimaryZip,
	Cell = p.CellPhone,
	Home = p.HomePhone,
	p.EmailAddress,
	p.EmailAddress2,
	PhonesOk = CONVERT(BIT, IIF(p.DoNotPublishPhones = 1, 0, 1)),
	pp.MediumId,
	pp.SmallId,
	pp.X,
	pp.Y,
	PicDate = pp.CreatedDate,
	p.GenderId
FROM dbo.TagPerson tp 
JOIN dbo.People p ON p.PeopleId = tp.PeopleId
JOIN dbo.Picture pp ON pp.PictureId = p.PictureId 
WHERE tp.Id = @p1
