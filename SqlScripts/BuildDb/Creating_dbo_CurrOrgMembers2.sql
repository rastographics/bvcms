CREATE FUNCTION [dbo].[CurrOrgMembers2]
(
	@orgs VARCHAR(MAX), @pids VARCHAR(MAX)
)
RETURNS @tt TABLE
(
		OrganizationId int NOT NULL,
		OrganizationName nvarchar(100) NOT NULL,
		FirstName nvarchar(25),
		LastName nvarchar(100) NOT NULL,
		Gender nvarchar(20),
		Grade int,
		ShirtSize nvarchar(20),
		Request nvarchar(140),
		Amount money NOT NULL,
		AmountPaid money NOT NULL,
		HasBalance int NOT NULL,
		Groups NVARCHAR(MAX),
		Email nvarchar(150),
		HomePhone nvarchar(30),
		CellPhone NVARCHAR(30),
		WorkPhone NVARCHAR(30),
		Age int,
		BirthDate datetime,
		JoinDate datetime,
		MemberStatus nvarchar(50),
		SchoolOther nvarchar(100),
		LastAttend datetime,
		AttendPct real,
		AttendStr nvarchar(200),
		MemberType nvarchar(100),
		MemberInfo NVARCHAR(MAX),
		Questions NVARCHAR(MAX),
		InactiveDate datetime,
		Medical nvarchar(1000),
		PeopleId int NOT NULL,
		EnrollDate datetime,
		Tickets int
)
AS
BEGIN

	DECLARE @t TABLE (id INT)
	INSERT INTO @t SELECT i.Value FROM dbo.SplitInts(@orgs) i

	DECLARE @pt TABLE (id INT)
	INSERT INTO @pt SELECT i.Value FROM dbo.SplitInts(@pids) i

	INSERT INTO @tt
	SELECT 
	om.OrganizationId,
	o.OrganizationName,
	p.PreferredName FirstName,
	p.LastName,
	g.Code Gender,
	p.Grade,
	om.ShirtSize,
	om.Request,
	ISNULL(ts.IndAmt, 0) Amount,
	ISNULL(ts.IndPaid, 0) AmountPaid,
	CASE WHEN ISNULL(ts.IndDue, 0) > 0 THEN 1 ELSE 0 END HasBalance, 
	(SELECT Stuff(
	  (SELECT N', ' + mt.Name 
		FROM dbo.OrgMemMemTags omt 
		JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId 
		WHERE omt.OrgId = om.OrganizationId AND omt.PeopleId = om.PeopleId
		FOR XML PATH(''),TYPE)
	  .value('text()[1]','nvarchar(max)'),1,2,N'')) Groups,
	p.EmailAddress Email,
	dbo.FmtPhone(p.HomePhone) HomePhone,
	dbo.FmtPhone(p.CellPhone) CellPhone,
	dbo.FmtPhone(p.WorkPhone) WorkPhone,
	p.Age,
	DATETIMEFROMPARTS(p.BirthYear, p.BirthMonth, p.BirthDay, 0, 0, 0, 0) BirthDate,
	p.JoinDate,
	ms.Description MemberStatus,
	p.SchoolOther,
	om.LastAttended LastAttend,
	om.AttendPct,
	om.AttendStr,
	mt.Description MemberType,
	MemberInfo = om.UserData,

	(SELECT STUFF((
		SELECT CHAR(13) + '    ' + Question + ': ' + Answer
		FROM dbo.OnlineRegQA
		WHERE type IN ('text', 'question')
		AND OrganizationId = om.OrganizationId
		AND PeopleId = om.PeopleId
		FOR XML PATH(''), TYPE
	).value('.', 'varchar(max)'), 1, 1, '')),

	om.InactiveDate,
	rr.MedicalDescription Medical,
	p.PeopleId,
	om.EnrollmentDate EnrollDate,
	om.Tickets
FROM dbo.OrganizationMembers om
JOIN @pt pt ON pt.id = om.PeopleId
JOIN dbo.People p ON p.PeopleId = om.PeopleId
JOIN dbo.Organizations o ON o.OrganizationId = om.OrganizationId
JOIN lookup.Gender g ON g.Id = p.GenderId
JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
JOIN lookup.MemberType mt ON mt.Id = om.MemberTypeId
LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId
LEFT JOIN dbo.TransactionSummary ts ON ts.OrganizationId = om.OrganizationId AND ts.PeopleId = om.PeopleId
WHERE om.OrganizationId IN (SELECT id FROM @t)
	RETURN 
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
