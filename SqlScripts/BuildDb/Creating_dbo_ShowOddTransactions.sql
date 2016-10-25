CREATE PROCEDURE [dbo].[ShowOddTransactions]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @t TABLE(PeopleId INT, OrgId INT)

	DECLARE @tid INT, @typeid INT, @orgid INT, @pid INT
	DECLARE @ptid INT = 0, @ptypeid INT = 0, @porgid INT = 0, @ppid INT = 0

	DECLARE c CURSOR FOR
	SELECT TransactionId, TransactionTypeId, OrganizationId, PeopleId 
	FROM dbo.EnrollmentTransaction
	ORDER BY PeopleId, OrganizationId, TransactionId

	OPEN c
	FETCH NEXT FROM c INTO @tid, @typeid, @orgid, @pid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @ppid = @pid AND @porgid = @orgid
		BEGIN
			IF (@typeid < 3 AND @ptypeid < 3) or (@typeid > 3 AND @ptypeid > 3)
				INSERT @t (PeopleId, OrgId) VALUES (@pid, @orgid)
		END
		
		SELECT @ptid = @tid, @ptypeid = @typeid, @porgid = @orgid, @ppid = @pid
		FETCH NEXT FROM c INTO @tid, @typeid, @orgid, @pid
	END
	CLOSE c
	DEALLOCATE c
	
	DECLARE cc CURSOR FOR
	SELECT DISTINCT * FROM @t
	OPEN cc
	FETCH NEXT FROM cc INTO @pid, @orgid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC dbo.ShowTransactions @pid, @orgid
		FETCH NEXT FROM cc INTO @pid, @orgid
	END
	CLOSE cc
	DEALLOCATE cc
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
