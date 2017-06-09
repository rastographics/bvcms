CREATE PROCEDURE [dbo].[DropOrgMemberAddET](@oid INT, @pid INT, @dropdate DATETIME)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @attendcount INT
	DECLARE @daystoignorehistory INT
	DECLARE @enrolled DATETIME
	DECLARE @created DATETIME
	DECLARE @orgname nvarchar(100)
	DECLARE @attpct REAL
	DECLARE @membertype INT
	DECLARE @pending BIT

	SELECT 
		@enrolled = EnrollmentDate,
		@created = CreatedDate,
		@attpct = AttendPct,
		@membertype = MemberTypeId,
		@pending = Pending
	FROM dbo.OrganizationMembers om
	WHERE om.PeopleId = @pid AND om.OrganizationId = @oid
	
	IF(@membertype IS NULL)
		RETURN 0

	SELECT @attendcount = (SELECT COUNT(*) FROM dbo.Attend a
						   JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
						   WHERE m.OrganizationId = o.OrganizationId
						   AND a.PeopleId = @pid
						   AND (a.MeetingDate < GETDATE() OR a.AttendanceFlag = 1)),
		   @daystoignorehistory = o.DaysToIgnoreHistory,
		   @orgname = OrganizationName
	FROM dbo.Organizations o
	WHERE OrganizationId = @oid

	IF(@enrolled IS NULL)
		SET @enrolled = @created

	INSERT dbo.EnrollmentTransaction
	        ( CreatedBy ,
	          CreatedDate ,
	          TransactionDate ,
	          TransactionTypeId ,
	          OrganizationId ,
	          OrganizationName ,
	          PeopleId ,
	          MemberTypeId ,
	          AttendancePercentage ,
	          Pending
	        )
	VALUES  ( 0 , -- CreatedBy - int
	          GETDATE() , -- CreatedDate - datetime
	          @dropdate , -- TransactionDate - datetime
	          5 , -- TransactionTypeId - int
	          @oid , -- OrganizationId - int
	          @orgname , -- OrganizationName - nvarchar(100)
	          @pid , -- PeopleId - int
	          @membertype , -- MemberTypeId - int
	          @attpct , -- AttendancePercentage - real
	          @pending  -- Pending - bit
	        )
	DECLARE @tranid INT = SCOPE_IDENTITY()
	        
	DELETE dbo.OrgMemMemTags 
	WHERE PeopleId = @pid AND OrgId = @oid

	DELETE dbo.OrgMemberExtra
	WHERE PeopleId = @pid AND OrganizationId = @oid

	DELETE dbo.OrganizationMembers 
	WHERE PeopleId = @pid AND OrganizationId = @oid

	DELETE dbo.SubRequest 
	WHERE EXISTS(SELECT NULL FROM Attend a 
				 WHERE a.AttendId = AttendId 
				 AND a.OrganizationId = @oid 
				 AND a.MeetingDate > GETDATE() 
				 AND a.PeopleId = @pid)
				 
	DELETE dbo.Attend 
	WHERE OrganizationId = @oid 
	AND MeetingDate > GETDATE() 
	AND PeopleId = @pid 
	AND ISNULL(Commitment, 1) = 1

	RETURN @tranid
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
