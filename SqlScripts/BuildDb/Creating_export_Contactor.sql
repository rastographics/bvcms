CREATE VIEW [export].[Contactor] AS 
SELECT 
	ContactId ,
    PeopleId 
FROM dbo.Contactors
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
