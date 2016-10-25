CREATE VIEW [dbo].[FirstName]
AS
SELECT     FirstName, COUNT(*) AS count
FROM         dbo.People
GROUP BY FirstName
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
