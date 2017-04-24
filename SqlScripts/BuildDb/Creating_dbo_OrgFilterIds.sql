
CREATE FUNCTION [dbo].[OrgFilterIds](
	 @queryid UNIQUEIDENTIFIER
) RETURNS TABLE
AS
RETURN
(
	SELECT PeopleId
	FROM dbo.OrgFilterPeople(@queryid, NULL)
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
