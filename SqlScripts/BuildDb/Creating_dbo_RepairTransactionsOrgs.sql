CREATE PROCEDURE [dbo].[RepairTransactionsOrgs] (@orgid INT)
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
		FETCH NEXT FROM cur3 INTO @pid
	END
	CLOSE cur3
	DEALLOCATE cur3
END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
