CREATE PROCEDURE [dbo].[ChangedGiving](@dt1 DATETIME, @dt2 DATETIME, @lopct FLOAT, @hipct FLOAT)
AS
BEGIN

	SELECT PeopleId, Name, dbo.ContributionAmount2(PeopleId, @dt1, @dt2, NULL) FROM dbo.People
	WHERE dbo.ContributionChange(PeopleId, @dt1, @dt2) BETWEEN @lopct AND @hipct
	
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
