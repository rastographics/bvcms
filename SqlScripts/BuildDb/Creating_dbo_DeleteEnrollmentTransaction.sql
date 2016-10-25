CREATE PROC [dbo].[DeleteEnrollmentTransaction](@id INT) 
AS  
BEGIN 
	DECLARE @oid INT, @pid INT 
 
	SELECT @oid = OrganizationId, @pid = PeopleId FROM dbo.EnrollmentTransaction WHERE TransactionId = @id 
	UPDATE dbo.EnrollmentTransaction 
	SET EnrollmentTransactionId = NULL 
	WHERE OrganizationId = @oid AND PeopleId = @pid 
 
	DELETE dbo.EnrollmentTransaction 
	WHERE TransactionId = @id 
 
	EXEC dbo.RepairEnrollmentTransaction @oid, @pid 
	 
END 
 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
