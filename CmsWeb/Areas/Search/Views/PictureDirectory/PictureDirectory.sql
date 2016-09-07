SELECT
	p.PeopleId,
	p.LastName,
	p.FamilyId,
	p.Name,
	p.PreferredName,
	p.SuffixCode,
	Birthday = dbo.DOB(p.BirthMonth,p.BirthDay, p.BirthYear),
	[Address] = p.PrimaryAddress,
	Address2 = p.PrimaryAddress2,
	City = p.PrimaryCity,
	St = p.PrimaryState,
	Zip = p.PrimaryZip,
	Cell = p.CellPhone,
	Home = p.HomePhone,
	p.EmailAddress,
	p.EmailAddress2,
	PhonesOk = ISNULL(p.DoNotPublishPhones, 0),
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
