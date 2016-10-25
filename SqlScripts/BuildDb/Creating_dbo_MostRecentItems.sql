-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[MostRecentItems] (@uid INT)
RETURNS @t TABLE (Id int, Name nvarchar(150),[Type] nvarchar(3))
AS
BEGIN
	DECLARE @dt7 DateTime = DATEADD(DAY, -7, GETDATE())
	DECLARE @pid INT
	SELECT @pid = PeopleId FROM Users WHERE UserId = @uid
	INSERT INTO @t
	SELECT TOP 5 OrgId [Id], OrganizationName [Name], 'org' [Type]
	FROM (SELECT
	         ROW_NUMBER() OVER ( PARTITION BY OrgId ORDER BY ActivityDate DESC ) AS [RowNumber],
	         OrgId,
	         OrganizationName,
	         ActivityDate
	      FROM dbo.ActivityLog a
	      JOIN dbo.Organizations o ON a.OrgId = o.OrganizationId
	      WHERE OrgId IS NOT NULL 
	      AND UserId = @uid 
	      ) t
	WHERE RowNumber = 1
	ORDER BY t.ActivityDate DESC

	INSERT INTO @t
	SELECT TOP 5 PeopleId [Id], Name, 'per' [Type]
	FROM (SELECT
	         ROW_NUMBER() OVER ( PARTITION BY a.PeopleId ORDER BY ActivityDate DESC ) AS [RowNumber],
	         a.PeopleId,
	         Name,
	         ActivityDate
	      FROM dbo.ActivityLog a
	      JOIN dbo.People p ON a.PeopleId = p.PeopleId
	      WHERE a.PeopleId IS NOT NULL 
	      AND a.PeopleId <> @pid
	      AND UserId = @uid 
	      ) t
	WHERE RowNumber = 1
	ORDER BY t.ActivityDate DESC
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
