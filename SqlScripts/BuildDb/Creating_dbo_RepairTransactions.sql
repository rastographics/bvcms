CREATE PROCEDURE [dbo].[RepairTransactions] (@orgid INT)
AS
BEGIN
	INSERT dbo.EnrollmentTransaction
        ( TransactionStatus ,
          CreatedDate ,
          TransactionDate ,
          TransactionTypeId ,
          OrganizationId ,
          OrganizationName ,
          PeopleId ,
          MemberTypeId ,
          EnrollmentDate ,
          AttendancePercentage ,
          Pending ,
          InactiveDate ,
          UserData ,
          Request ,
          ShirtSize ,
          Grade ,
          Tickets ,
          RegisterEmail ,
          Score ,
          SmallGroups
        )
	SELECT 
		0,
		GETDATE(),
		DATEADD(mi,5, TransactionDate),
		5, -- drop
		OrganizationId,
		OrganizationName,
        PeopleId ,
        MemberTypeId ,
        EnrollmentDate ,
        AttendancePercentage ,
        Pending ,
        InactiveDate ,
        UserData ,
        Request ,
        ShirtSize ,
        Grade ,
        Tickets ,
        RegisterEmail ,
        Score ,
        SmallGroups
	FROM dbo.EnrollmentTransaction et
	WHERE et.OrganizationId = @orgid
	AND et.TransactionTypeId = 1 -- join
	AND NOT EXISTS(SELECT NULL 
		FROM dbo.OrganizationMembers 
		WHERE OrganizationId = et.OrganizationId 
		AND PeopleId = et.PeopleId)
	AND et.TransactionDate = 
		(SELECT MAX(TransactionDate) 
		FROM dbo.EnrollmentTransaction 
		WHERE PeopleId = et.PeopleId 
		AND OrganizationId = et.OrganizationId)

	EXEC dbo.RepairEnrollmentTransactions @orgid
	
	DECLARE @id INT 
	SELECT TOP 1 @id = id FROM dbo.LongRunningOp
	WHERE id = @orgid
	AND operation = 'RepairTransactions'
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
		UPDATE dbo.LongRunningOp
		SET processed = ISNULL(processed,0) + 1
		WHERE id = @id
		FETCH NEXT FROM cur3 INTO @pid
	END
	CLOSE cur3
	DEALLOCATE cur3
	
	UPDATE dbo.LongRunningOp
	SET completed = GETDATE()
	WHERE id = @id
	AND operation = 'RepairTransactions'
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
