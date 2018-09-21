CREATE VIEW [export].[XpContactor] AS 
SELECT 
	ContactId ,
    PeopleId 
FROM dbo.Contactors
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
