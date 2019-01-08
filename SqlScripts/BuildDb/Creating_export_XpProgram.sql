CREATE VIEW [export].[XpProgram] AS 
SELECT 
	Id ,
    [Name]
FROM dbo.Program
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
