
ALTER VIEW [dbo].[AppRegistrations]
AS

WITH appreg AS (
	SELECT Body 
	FROM dbo.Content 
	WHERE Name = 'AppRegistrations' AND TypeID = 1
),
reg AS (
	SELECT o.OrganizationId
		 , o.RegistrationTitle Title
		 , o.OrganizationName
		 , o.Description
		 , CASE WHEN ISNULL(o.AppCategory, '') <> '' THEN o.AppCategory ELSE 'Other' END AppCategory
		 , o.PublicSortOrder
		 , o.UseRegisterLink2
		 , o.RegStart
		 , o.RegEnd
	FROM dbo.Organizations o 
	JOIN dbo.ActiveRegistrations ON dbo.ActiveRegistrations.OrganizationId = o.OrganizationId
	WHERE (ISNULL(o.AppCategory, '') <> 'InvitationOnly') AND o.RegStart IS NOT NULL AND o.RegEnd > GETDATE() AND o.OrganizationStatusId = 30
),
idx AS (
	SELECT AppCategory, txt.Body,
		PATINDEX('%' + reg.AppCategory + ' %', txt.Body) + LEN(reg.AppCategory) [Start],
		CHARINDEX(CHAR(10), txt.Body, PATINDEX('%' + reg.AppCategory + ' %', txt.Body)) + LEN(reg.AppCategory) [End],
		LEN(reg.AppCategory) [Length]
	FROM reg
	CROSS APPLY (select TOP(1) Body FROM appreg) AS txt
)
SELECT reg.*, LTRIM(RTRIM(SUBSTRING(idx.Body, idx.[Start], idx.[End] - idx.[Start] - idx.[Length]))) LongAppCategory
FROM reg
LEFT JOIN idx ON reg.AppCategory = idx.AppCategory