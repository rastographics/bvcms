
CREATE TRIGGER [dbo].[insMeeting] 
   ON  [dbo].[Meetings]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @oid INT,
			@mid INT,
			@mdt DATETIME,
			@mbr INT,
			@atyp INT,
			@grp INT,
			@pid INT,
			@offsite INT,
			@acr INT,
			@NoAutoAbsents BIT,
			@firstdt DATETIME,
			@lastdt DATETIME
	
	SELECT 
		@oid = OrganizationId,
		@mid = MeetingId,
		@mdt = MeetingDate,
		@grp = GroupMeetingFlag,
		@acr = AttendCreditId,
		@NoAutoAbsents = NoAutoAbsents
	FROM INSERTED
	
	IF (@NoAutoAbsents = 1)
		RETURN
	
	SELECT @NoAutoAbsents = NoAutoAbsents, @firstdt = FirstMeetingDate, @lastdt = DATEADD(d, 1, LastMeetingDate)
	FROM dbo.Organizations WHERE OrganizationId = @oid
	IF (@NoAutoAbsents = 1)
		RETURN
		
	DECLARE c CURSOR FOR
	SELECT PeopleId, MemberTypeId, mt.AttendanceTypeId 
	FROM dbo.OrganizationMembers m
	JOIN lookup.MemberType mt ON m.MemberTypeId = mt.Id
	WHERE OrganizationId = @oid
	AND EnrollmentDate < @mdt
	AND @acr IS NOT NULL
	AND m.MemberTypeId <> 230 -- Inactive
	AND m.MemberTypeId <> 311 -- Prospect
	AND ISNULL(m.Pending, 0) = 0
	
	OPEN c;
	FETCH NEXT FROM c INTO @pid, @mbr, @atyp;
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		
		IF (@firstdt IS NULL OR @mdt > @firstdt) AND (@lastdt IS NULL OR @mdt < @lastdt)
			SELECT @offsite = CASE WHEN exists(	
					SELECT NULL FROM dbo.OrganizationMembers om
					JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
					WHERE om.PeopleId = @pid
					AND om.OrganizationId <> @oid
					AND o.Offsite = 1
					AND o.FirstMeetingDate <= @mdt
					AND @mdt <= DATEADD(d, 1, o.LastMeetingDate))
				THEN 1 ELSE 0 END
		ELSE
			SET @offsite = 0
			
		INSERT INTO dbo.Attend
		        ( PeopleId ,
		          MeetingId ,
		          OrganizationId ,
		          MeetingDate ,
		          MemberTypeId ,
		          AttendanceTypeId ,
		          CreatedDate ,
		          AttendanceFlag ,
		          OtherAttends 
		        )
		VALUES  ( @pid , -- PeopleId - int
		          @mid , -- MeetingId - int
		          @oid , -- OrganizationId - int
		          @mdt , -- MeetingDate - datetime
		          @mbr , -- MemberTypeId - int
		          CASE WHEN @grp = 1 THEN 90 WHEN @offsite = 1 THEN 80 ELSE @atyp END , -- AttendanceTypeId - int
		          GETDATE() , -- CreatedDate - datetime
		          0 , -- AttendanceFlag - bit
		          @offsite
		        )
		FETCH NEXT FROM c INTO @pid, @mbr, @atyp;
	END;
	CLOSE c;
	DEALLOCATE c;
	IF @acr IS NOT NULL
	BEGIN
		DECLARE @usebroker bit = (SELECT is_broker_enabled FROM sys.databases WHERE name = DB_NAME())
		IF @usebroker = 1
		BEGIN
			DECLARE @dialog uniqueidentifier
			BEGIN DIALOG CONVERSATION @dialog
			  FROM SERVICE UpdateAttendStrService
			  TO SERVICE 'UpdateAttendStrService'
			  ON CONTRACT UpdateAttendStrContract
			  WITH ENCRYPTION = OFF;
			SEND ON CONVERSATION @dialog MESSAGE TYPE UpdateAttendStrMessage (@oid)
		END
		ELSE
			EXEC dbo.UpdateAllAttendStr @oid
			
		
	END
	
	
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
