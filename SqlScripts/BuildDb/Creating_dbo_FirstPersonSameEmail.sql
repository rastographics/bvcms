CREATE VIEW [dbo].[FirstPersonSameEmail] AS
SELECT MIN(PeopleId) PeopleId
FROM dbo.People
GROUP BY EmailAddress
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
