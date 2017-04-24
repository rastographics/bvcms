CREATE FUNCTION [dbo].[OrgFilterCheckedCount](@queryid UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
	DECLARE @ret INT

			SELECT @ret = COUNT(*)
			FROM dbo.TagPerson tp
			JOIN dbo.Tag t ON t.Id = tp.Id
			LEFT JOIN dbo.OrgFilterIds(@queryid) op ON op.PeopleId = tp.PeopleId
			WHERE t.TypeId = 3 AND t.Name = CONVERT(VARCHAR(50), @queryid)
			AND op.PeopleId IS NOT NULL

	RETURN @ret

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
