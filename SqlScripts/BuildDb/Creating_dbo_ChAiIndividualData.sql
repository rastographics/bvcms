CREATE VIEW [dbo].[ChAiIndividualData]
AS
SELECT
	PeopleId
	,f.FamilyId
	,p.CreatedDate
	,f.HeadOfHouseholdId
	,FamilyCreatedDate = f.CreatedDate
	,MemberStatus = mb.Description
	,Salutation = p.TitleCode
	,FirstName = p.PreferredName
	,p.LastName
	,FamilyPosition = fp.Description
	,MaritalStatus = ms.Description
	,Gender = g.Description
	,Address1 = p.PrimaryAddress
	,Address2 = p.PrimaryAddress2
	,City = p.PrimaryCity
	,State = p.PrimaryState
	,Zip = SUBSTRING(ISNULL(p.PrimaryZip, ''), 1, 5)
	,HomePhone = dbo.FmtPhone(f.HomePhone)
	,p.EmailAddress
	,BirthDate = dbo.DOB(p.BirthMonth, p.BirthDay, p.BirthYear)
	,LastModified = (SELECT MAX(cl.Created)
				FROM dbo.ChangeLog cl
				WHERE cl.PeopleId = p.PeopleId)
FROM dbo.People p
JOIN dbo.Families f ON f.FamilyId = p.FamilyId
JOIN lookup.FamilyPosition fp ON fp.Id = p.PositionInFamilyId
JOIN lookup.MaritalStatus ms ON ms.Id = p.MaritalStatusId
JOIN lookup.Gender g ON g.Id = p.GenderId
JOIN lookup.MemberStatus mb ON mb.Id = p.MemberStatusId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
