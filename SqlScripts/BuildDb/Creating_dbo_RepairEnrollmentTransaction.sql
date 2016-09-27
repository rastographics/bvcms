CREATE PROC [dbo].[RepairEnrollmentTransaction](@oid INT, @pid INT) 
AS  
BEGIN 
	DECLARE @t TABLE (id INT, typ INT, dt DATETIME) 
	INSERT @t (id, typ, dt) 
	SELECT TransactionId, TransactionTypeId, TransactionDate 
	FROM dbo.EnrollmentTransaction  
	WHERE OrganizationId = @oid AND PeopleId = @pid 
 
	UPDATE dbo.EnrollmentTransaction 
	SET NextTranChangeDate = (SELECT TOP 1 dt FROM @t WHERE e.TransactionTypeId <= 3 AND dt > e.TransactionDate ORDER BY dt)  
	  , EnrollmentTransactionId = (SELECT TOP 1 id FROM @t WHERE typ <= 2 AND dt < e.TransactionDate AND e.TransactionTypeId >= 3 ORDER BY dt DESC)  
	FROM dbo.EnrollmentTransaction e 
	WHERE e.OrganizationId = @oid AND e.PeopleId = @pid 
END 
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
