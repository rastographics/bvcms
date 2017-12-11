CREATE VIEW [export].[Contactee] AS 
SELECT 
	ContactId ,
    PeopleId 
FROM dbo.Contactees
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
