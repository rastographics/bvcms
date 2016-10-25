CREATE PROCEDURE [dbo].[SpamReporterRemove](@email VARCHAR(100))
AS
BEGIN
	
	IF @email IS NOT NULL AND @email LIKE '%@%'
	BEGIN
		DECLARE @pid INT
		DECLARE @dt DATETIME = GETDATE()
		DECLARE c CURSOR FOR
		SELECT PeopleId
		FROM dbo.People
		WHERE EmailAddress = @email
		OPEN c
		FETCH NEXT FROM c INTO @pid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC dbo.AddEditExtraDate @pid, 'SpamReporterRemoved', @dt
			FETCH NEXT FROM c INTO @pid
		END
		CLOSE c
		DEALLOCATE c
	END
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
