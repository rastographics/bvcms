CREATE VIEW [dbo].[RegistrationList]
AS

SELECT 
	 tt.Id
	,tt.Stamp
	,o.OrganizationId 
	,o.OrganizationName 
	,pu.PeopleId
	,pu.Name
	,data.value('(/OnlineRegModel/List/OnlineRegPersonModel)[1]/DateOfBirth[1]', 'varchar(50)') [dob]
	,data.value('(/OnlineRegModel/List/OnlineRegPersonModel)[1]/FirstName[1]', 'varchar(50)') [first]
	,data.value('(/OnlineRegModel/List/OnlineRegPersonModel)[1]/LastName[1]', 'varchar(50)') [last]
	,data.value('count(/OnlineRegModel/List/OnlineRegPersonModel)', 'int') cnt
	,cast (CASE WHEN data.value('(/OnlineRegModel/URL)[1]', 'varchar(100)') LIKE '%source=%' THEN 1 ELSE 0 END AS BIT) [mobile]
	,tt.completed
	,tt.abandoned
	,tt.UserPeopleId
	,CONVERT(BIT, CASE WHEN tt.Stamp < o.RegStart THEN 1 ELSE 0 END) expired
	,o.RegStart
	,o.RegEnd
FROM 
(
	SELECT 
		Id
		, data
		, d.Stamp
		, d.completed
		, d.abandoned
		, d.UserPeopleId
		, d.OrganizationId
	FROM dbo.RegistrationData d
) tt
LEFT JOIN dbo.Organizations o ON o.OrganizationId = tt.OrganizationId
LEFT JOIN dbo.People pu ON pu.PeopleId = tt.UserPeopleId
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
