CREATE PROCEDURE [dbo].[PopulateComputedEnrollmentTransactions] (@orgid INT)
AS
BEGIN
	INSERT INTO dbo.RepairTransactionsRun(started, processed, orgid, count)
	VALUES(GETDATE(), 0, @orgid, 
		(SELECT COUNT(*) FROM dbo.OrganizationMembers WHERE OrganizationId = @orgid))
	        
	UPDATE dbo.EnrollmentTransaction
	SET NextTranChangeDate = dbo.NextTranChangeDate(PeopleId, OrganizationId, TransactionDate, TransactionTypeId),
		EnrollmentTransactionId = dbo.EnrollmentTransactionId(PeopleId, OrganizationId, TransactionDate, TransactionTypeId)
	WHERE OrganizationId = @orgid
	        
	DECLARE @id INT 
	SELECT TOP 1 @id = id FROM dbo.RepairTransactionsRun
	WHERE orgid = @orgid
	ORDER BY id DESC
	
	DECLARE cur3 CURSOR FOR 
	SELECT PeopleId 
	FROM dbo.OrganizationMembers
	WHERE OrganizationId = @orgid
	OPEN cur3
	DECLARE @pid INT, @n INT
	FETCH NEXT FROM cur3 INTO @pid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXECUTE dbo.UpdateAttendStr @orgid, @pid
		UPDATE dbo.RepairTransactionsRun
		SET processed = processed + 1
		WHERE id = @id
		FETCH NEXT FROM cur3 INTO @pid
	END
	CLOSE cur3
	DEALLOCATE cur3
	
	UPDATE dbo.RepairTransactionsRun
	SET completed = GETDATE()
	WHERE id = @id
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
