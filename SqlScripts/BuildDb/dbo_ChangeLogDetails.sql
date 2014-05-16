CREATE VIEW [dbo].ChangeLogDetails
AS
SELECT 
	lg.Id,
	lg.PeopleId,
	lg.Field Section,
	lg.Created,
	lg.FamilyId,
	lg.UserPeopleId,
	dt.Field,
	dt.Before,
	dt.After
FROM dbo.ChangeLog lg
JOIN dbo.ChangeDetails dt ON dt.Id = lg.Id


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
