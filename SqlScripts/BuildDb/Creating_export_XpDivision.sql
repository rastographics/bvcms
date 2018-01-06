CREATE VIEW [export].[XpDivision] AS 
SELECT 
	Id ,
	[Name] ,
	ProgId 
FROM dbo.Division
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
