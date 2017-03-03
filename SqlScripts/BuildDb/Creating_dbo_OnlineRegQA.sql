
CREATE VIEW [dbo].[OnlineRegQA]
AS

WITH 
questions AS (
	SELECT
		om.OrganizationId,
		om.PeopleId,
		pref.value('(@set)[1]', 'int') AS [set],
		pref.value('(@question)[1]', 'nvarchar(500)') AS Question,
		pref.value('(text())[1]', 'nvarchar(500)') AS Answer
	FROM  
	      dbo.OrganizationMembers om CROSS APPLY
	      OnlineRegData.nodes('/OnlineRegPersonModel/ExtraQuestion') AS Question(pref)
	WHERE om.OnlineRegData IS NOT NULL
)
, textquestions AS (
	SELECT
		om.OrganizationId,
		om.PeopleId,
		pref.value('(@set)[1]', 'int') AS [set],
		pref.value('(@question)[1]', 'nvarchar(500)') AS Question,
		pref.value('(text())[1]', 'nvarchar(max)') AS Answer
	FROM  
	      dbo.OrganizationMembers om CROSS APPLY
	      OnlineRegData.nodes('/OnlineRegPersonModel/Text') AS Question(pref)
	WHERE om.OnlineRegData IS NOT NULL
)
, yesno AS (
	SELECT
		om.OrganizationId,
		om.PeopleId,
		NULL AS [set],
		pref.value('(@question)[1]', 'nvarchar(500)') AS Question,
		pref.value('(text())[1]', 'nvarchar(500)') AS Answer
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
		[set],
		Question,
		Answer
	FROM questions
	UNION
	SELECT 
		'text' [type],
		PeopleId,
		OrganizationId,
		[set],
		Question,
		Answer
	FROM textquestions
	UNION
    SELECT 
		'yesno',
		PeopleId,
		OrganizationId,
		[set],
		Question,
		Answer
	FROM yesno
)
SELECT 
	OrganizationId,
	p.PeopleId,
	[type],
	[set],
	Question,
	Answer
FROM results
JOIN dbo.People p ON p.PeopleId = results.PeopleId






GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
