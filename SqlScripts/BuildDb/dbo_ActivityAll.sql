
CREATE VIEW [dbo].[ActivityAll]
	AS
	SELECT Machine, ActivityDate, ISNULL(u.Name, '') Name, ISNULL(g.UserId, 0) UserId, g.Activity FROM dbo.ActivityLog g
	LEFT JOIN dbo.Users u ON g.UserId = u.UserId
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
