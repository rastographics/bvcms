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
	
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
