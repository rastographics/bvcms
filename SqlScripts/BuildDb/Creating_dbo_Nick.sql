CREATE VIEW [dbo].[Nick]
AS
SELECT NickName, COUNT(*) AS [count] FROM dbo.People GROUP BY NickName
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
