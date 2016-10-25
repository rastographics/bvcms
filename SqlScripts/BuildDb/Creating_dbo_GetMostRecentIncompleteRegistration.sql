-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetMostRecentIncompleteRegistration](@oid INT, @pid INT)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @id INT
	
	DECLARE @t TABLE (id INT, row INT)

	INSERT INTO @t ( id, row )
		SELECT 
			e.Id, ROW_NUMBER() OVER (ORDER BY Stamp DESC) AS row
		FROM dbo.RegistrationData e
		JOIN dbo.Organizations o ON o.OrganizationId = e.OrganizationId
		WHERE e.OrganizationId = @oid
		AND UserPeopleId = @pid
		AND Stamp > ISNULL(o.RegStart, DATEADD(DAY, -30, GETDATE()))
		AND ISNULL(e.abandoned, 0) = 0
		AND ISNULL(e.completed, 0) = 0

	UPDATE dbo.RegistrationData
	SET abandoned = 1
	FROM dbo.RegistrationData d
	JOIN @t ON [@t].id = d.Id
	WHERE row > 1

	SELECT @id = id FROM @t
	WHERE row = 1

	RETURN @id

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
