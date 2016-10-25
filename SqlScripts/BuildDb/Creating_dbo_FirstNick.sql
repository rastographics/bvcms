CREATE VIEW [dbo].[FirstNick]
AS
SELECT FirstName, NickName, COUNT(*) AS [count] FROM dbo.People GROUP BY FirstName, NickName
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
