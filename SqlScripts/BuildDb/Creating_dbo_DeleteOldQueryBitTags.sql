-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteOldQueryBitTags]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @t TABLE (id INT)
	;WITH flags AS (
		SELECT 
			name = dbo.RegexMatch(name, 'F[0-9]{2}')
		FROM dbo.Query
		WHERE name LIKE 'F[0-9][0-9]:%'
	),
	tags AS (
		SELECT *
		FROM dbo.Tag
		WHERE TypeId = 100
	), deletes AS (
		SELECT t.Id, t.name 
		FROM tags t
		LEFT OUTER JOIN flags f ON f.name = t.Name
		WHERE f.name IS NULL
	)
	INSERT INTO @t ( id )
	SELECT Id FROM deletes

	DELETE dbo.TagPerson
	WHERE Id IN (SELECT Id FROM @t)

	DELETE dbo.Tag
	WHERE Id IN (SELECT Id FROM @t)

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
