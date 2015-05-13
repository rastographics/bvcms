CREATE VIEW [dbo].[OnlineRegQA]
AS

WITH 
questions AS (
	SELECT
		om.OrganizationId,
		om.PeopleId,
		pref.value('(@question)[1]', 'varchar(500)') AS Question,
		pref.value('(text())[1]', 'varchar(500)') AS Answer
	FROM  
	      dbo.OrganizationMembers om CROSS APPLY
	      OnlineRegData.nodes('/OnlineRegPersonModel/ExtraQuestion') AS Question(pref)
	WHERE om.OnlineRegData IS NOT NULL
)
, textquestions AS (
	SELECT
		om.OrganizationId,
		om.PeopleId,
		pref.value('(@question)[1]', 'varchar(500)') AS Question,
		pref.value('(text())[1]', 'varchar(max)') AS Answer
	FROM  
	      dbo.OrganizationMembers om CROSS APPLY
	      OnlineRegData.nodes('/OnlineRegPersonModel/Text') AS Question(pref)
	WHERE om.OnlineRegData IS NOT NULL
)
, yesno AS (
	SELECT
		om.OrganizationId,
		om.PeopleId,
		pref.value('(@question)[1]', 'varchar(500)') AS Question,
		pref.value('(text())[1]', 'varchar(500)') AS Answer
	FROM  
	      dbo.OrganizationMembers om CROSS APPLY
	      OnlineRegData.nodes('/OnlineRegPersonModel/YesNoQuestion') AS Question(pref)
	WHERE om.OnlineRegData IS NOT NULL
)
, subgroups AS (
	SELECT
		om.OrganizationId,
		om.PeopleId,
		mt.NAME
	FROM  
		dbo.OrganizationMembers om CROSS APPLY
		dbo.MemberTags mt
		WHERE mt.OrgId = om.OrganizationId
		AND EXISTS(
			SELECT NULL FROM dbo.OrgMemMemTags omt
			WHERE omt.OrgId = om.OrganizationId
			AND omt.PeopleId = om.PeopleId
		)
)
, results AS (
	SELECT 
		'question' [type],
		PeopleId,
		OrganizationId,
		Question,
		Answer
	FROM questions
	UNION
	SELECT 
		'text' [type],
		PeopleId,
		OrganizationId,
		Question,
		Answer
	FROM textquestions
	UNION
    SELECT 
		'yesno',
		PeopleId,
		OrganizationId,
		Question,
		Answer
	FROM yesno
	UNION
    SELECT 
		'subgroup',
		PeopleId,
		OrganizationId,
		subgroups.NAME,
		'True'
	FROM subgroups
)
SELECT 
	OrganizationId,
	p.PeopleId,
	[type],
	Question,
	Answer
FROM results
JOIN dbo.People p ON p.PeopleId = results.PeopleId
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
