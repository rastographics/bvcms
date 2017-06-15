CREATE VIEW [dbo].[SearchNoDiacritics] AS 
(
	SELECT 
		PeopleId
		,FamilyId
		,EmailAddress
		,EmailAddress2
		,CellPhone
		,HomePhone
		,WorkPhone
		,BirthMonth
		,BirthDay
		,BirthYear
		,LastName = (SELECT CONVERT(VARCHAR, LastName) COLLATE SQL_Latin1_General_CP1253_CI_AI)
		,FirstName = (SELECT CONVERT(VARCHAR, FirstName) COLLATE SQL_Latin1_General_CP1253_CI_AI)
		,NickName = (SELECT CONVERT(VARCHAR, NickName) COLLATE SQL_Latin1_General_CP1253_CI_AI)
		,MiddleName = (SELECT CONVERT(VARCHAR, MiddleName) COLLATE SQL_Latin1_General_CP1253_CI_AI)
		,MaidenName = (SELECT CONVERT(VARCHAR, MaidenName) COLLATE SQL_Latin1_General_CP1253_CI_AI)
		,FirstName2 = (SELECT CONVERT(VARCHAR, FirstName2) COLLATE SQL_Latin1_General_CP1253_CI_AI)
	FROM dbo.People
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
