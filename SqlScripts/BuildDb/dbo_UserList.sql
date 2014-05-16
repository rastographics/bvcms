CREATE VIEW [dbo].[UserList]
AS
SELECT u.Username, u.UserId, u.Name, u.Name2, u.IsApproved, u.MustChangePassword, u.IsLockedOut, u.EmailAddress, ttt.LastActivityDate, u.PeopleId, dbo.UserRoleList(u.UserId) Roles FROM Users u
JOIN ( SELECT UserId, MAX(ActivityDate) AS LastActivityDate FROM dbo.ActivityLog GROUP BY UserId ) AS ttt
ON u.UserId = ttt.UserId
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
