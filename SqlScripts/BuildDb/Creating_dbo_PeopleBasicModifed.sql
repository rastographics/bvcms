
CREATE VIEW [dbo].[PeopleBasicModifed] AS 
SELECT 
	PeopleId
	,FamilyId
	,PositionInFamilyId
	,CampusId
	,FirstName
	,LastName
	,GenderId
	,p.BDate AS BirthDate
	,EmailAddress
	,MaritalStatusId
	,PrimaryAddress
	,PrimaryCity
	,PrimaryState
	,PrimaryZip
	,HomePhone
	,WorkPhone
	,CellPhone
	,CreatedDate
	,ModifiedDate = (
		SELECT TOP 1 Created 
		FROM dbo.ChangeLogDetails
		WHERE PeopleId = p.PeopleId
		ORDER BY Created DESC
	)
	,p.IsDeceased
FROM dbo.People p
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
