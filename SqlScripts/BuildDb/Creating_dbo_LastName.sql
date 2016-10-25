CREATE VIEW [dbo].[LastName]
AS
SELECT LastName, COUNT(*) AS [count] FROM dbo.People GROUP BY LastName
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
