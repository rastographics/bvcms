
IF OBJECT_ID('dbo.MergeCampuses') IS NULL
exec sp_execute '
CREATE PROC dbo.MergeCampuses(
	@destCampus int,
	@oldCampus int
) AS
BEGIN
	UPDATE dbo.Organizations
	SET CampusID = @destCampus
	WHERE CampusID = @oldCampus

	UPDATE dbo.People
	SET CampusID = @destCampus
	WHERE CampusID = @oldCampus

	UPDATE dbo.Contribution
	SET CampusID = @destCampus
	WHERE CampusID = @oldCampus

	UPDATE dbo.Resource
	SET CampusID = @destCampus
	WHERE CampusID = @oldCampus

	delete lookup.Campus
	WHERE Id = @oldCampus
	RETURN
END'
