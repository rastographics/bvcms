CREATE VIEW [dbo].[City]
AS
SELECT PrimaryCity AS City, PrimaryState AS State, PrimaryZip AS Zip, COUNT(*) AS [count] FROM dbo.People GROUP BY PrimaryCity, PrimaryState, PrimaryZip
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
