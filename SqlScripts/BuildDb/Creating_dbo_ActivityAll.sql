
CREATE VIEW [dbo].[ActivityAll]
	AS
	SELECT Machine, ActivityDate, ISNULL(u.Name, '') Name, ISNULL(g.UserId, 0) UserId, g.Activity FROM dbo.ActivityLog g
	LEFT JOIN dbo.Users u ON g.UserId = u.UserId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
