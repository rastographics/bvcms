CREATE FUNCTION [dbo].[OrgCheckedCount](@oid INT, @groupselect VARCHAR(30), @pid INT)
RETURNS INT
AS
BEGIN
	DECLARE @ret INT

			SELECT @ret = COUNT(*)
			FROM dbo.TagPerson tp
			JOIN dbo.Tag t ON t.Id = tp.Id
			LEFT JOIN dbo.OrgPeople(@oid, @groupselect, NULL, NULL, NULL, 1, NULL, NULL, 0, 0, 0, NULL) op ON op.PeopleId = tp.PeopleId
			WHERE t.TypeId = 10 AND t.Name = 'Org-' + CONVERT(VARCHAR, @oid)
			AND t.PeopleId = @pid
			AND op.PeopleId IS NOT NULL

	RETURN @ret

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
