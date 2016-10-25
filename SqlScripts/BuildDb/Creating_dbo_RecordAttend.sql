CREATE PROCEDURE [dbo].[RecordAttend]
( @MeetingId INT, @PeopleId INT, @Present BIT)
AS
BEGIN
	DECLARE @ret nvarchar(100)

	--BEGIN TRANSACTION
	
	DECLARE @orgid INT
			,@meetingdate DATETIME
			,@meetdt DATE
			,@dt DATETIME
			,@regularhour BIT
			,@membertypeid INT
			,@allowoverlap BIT
			,@bfcattend INT
			,@bfcid INT
			,@name nvarchar(50)
			,@bfclassid INT
			,@ResetNewVisitorDays INT

	SELECT
		@orgid = o.OrganizationId,
		@meetingdate = m.MeetingDate,
		@meetdt = CONVERT(DATE, m.MeetingDate),
		@allowoverlap = o.AllowAttendOverlap,
		@membertypeid = dbo.MemberTypeAsOf(o.OrganizationId, @PeopleId, m.MeetingDate)
	FROM dbo.Meetings m
	JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
	WHERE m.MeetingId = @MeetingId
	
	SELECT @ResetNewVisitorDays = ISNULL((SELECT Setting FROM dbo.Setting WHERE Id = 'ResetNewVisitorDays'), 180)
	SELECT @dt = DATEADD(DAY, -@ResetNewVisitorDays, @meetdt)
	
	SELECT @regularhour = CASE WHEN EXISTS(
		SELECT null
			FROM dbo.Meetings m
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE m.MeetingId = @MeetingId
				AND EXISTS(SELECT NULL FROM dbo.OrgSchedule
							WHERE OrganizationId = o.OrganizationId
							AND SchedDay IN (DATEPART(weekday, m.MeetingDate)-1, 10)
							AND CONVERT(TIME, m.MeetingDate) = CONVERT(TIME, MeetingTime)
							AND (o.FirstMeetingDate IS NULL OR o.FirstMeetingDate < @meetingdate) 
							AND (o.LastMeetingDate IS NULL OR o.LastMeetingDate > @meetingdate)
							)
		)
		THEN 1 ELSE 0 END

	IF @dt IS NULL
		SELECT @dt = DATEADD(DAY, 3 * -7, @meetdt)

	SELECT @name = p.[Name], @bfclassid = BibleFellowshipClassId
	FROM dbo.People p
	WHERE PeopleId = @PeopleId
	
	--Check Attended Elsewhere
	IF EXISTS(SELECT NULL
			FROM Attend ae
			JOIN dbo.Meetings m ON ae.MeetingId = m.MeetingId
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE ae.PeopleId = @PeopleId
			AND ae.AttendanceFlag = 1
			AND ae.MeetingDate = @meetingdate
			AND ae.OrganizationId <> @orgid
			AND o.AllowAttendOverlap <> 1
			AND @allowoverlap <> 1)
	BEGIN
		SELECT @ret = @name + '(' + CONVERT(nvarchar(15), @PeopleId) + ') already attended elsewhere'
		--ROLLBACK TRANSACTION
		SELECT @ret AS ret
		RETURN
	END
	
	-- BFC class membership at same hour
	SELECT TOP 1 @bfcid = om.OrganizationId 
	FROM dbo.OrganizationMembers om
	JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
	WHERE om.PeopleId = @PeopleId 
	AND om.OrganizationId <> @orgid
	AND om.MemberTypeId <> 230
	AND om.MemberTypeId <> 311
	AND ISNULL(om.Pending, 0) = 0
	AND @regularhour = 1
	AND EXISTS(SELECT NULL FROM dbo.OrgSchedule os
				JOIN lookup.AttendCredit at ON os.AttendCreditId = at.Id
				WHERE os.OrganizationId = o.OrganizationId
				AND os.SchedDay IN (DATEPART(weekday, @meetingdate)-1, 10)
				AND CONVERT(TIME, @meetingdate) = CONVERT(TIME, os.MeetingTime)
				AND at.Code = 'E' -- AttendCredit = Every Meeting vs Once a Week
				)

	-- BFC Attendance at same time
	SELECT TOP 1 @bfcattend = a.AttendId FROM dbo.Attend a
	JOIN dbo.OrganizationMembers om ON a.OrganizationId = om.OrganizationId AND a.PeopleId = om.PeopleId
	JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
	WHERE a.MeetingDate = @meetingdate
	AND @regularhour = 1
	AND EXISTS(SELECT NULL FROM dbo.OrgSchedule
				WHERE OrganizationId = o.OrganizationId
				AND SchedDay IN (DATEPART(weekday, @meetingdate)-1, 10)
				AND CONVERT(TIME, @meetingdate) = CONVERT(TIME, MeetingTime))
	AND om.OrganizationId <> @orgid
	AND om.OrganizationId = @bfcid
	AND a.PeopleId = @PeopleId

----------------------------------

	DECLARE @AttendId INT 
	SELECT @AttendId = AttendId 
	FROM dbo.Attend 
	WHERE MeetingId = @MeetingId AND PeopleId = @PeopleId
	
	IF @AttendId IS NULL
	BEGIN
		INSERT dbo.Attend
		        ( PeopleId ,
		          MeetingId ,
		          OrganizationId ,
		          MeetingDate ,
		          AttendanceFlag ,
		          CreatedDate,
		          MemberTypeId
		        )
		VALUES  ( @PeopleId,
		          @MeetingId,
		          @orgid,
		          @meetingdate,
		          @Present,
		          GETDATE(),
		          220
		        )
		SELECT @AttendId = SCOPE_IDENTITY()
	END
	ELSE
		UPDATE dbo.Attend
		SET AttendanceFlag = @Present, SeqNo = NULL
		WHERE AttendId = @AttendId
	
	DECLARE @OtherMeetings TABLE ( id INT )
	
	DECLARE @GroupMeetingFlag BIT
	SELECT @GroupMeetingFlag = GroupMeetingFlag 
	FROM dbo.Meetings 
	WHERE MeetingId = @MeetingId
	
	DECLARE @VIPAttendance TABLE ( id INT )
	
	INSERT INTO @VIPAttendance ( id ) SELECT AttendId
				FROM Attend v
				JOIN dbo.OrganizationMembers om ON v.OrganizationId = om.OrganizationId AND v.PeopleId = om.PeopleId
				WHERE v.PeopleId = @PeopleId
				AND v.MeetingDate = @meetingdate
				AND v.AttendanceFlag = 1
				AND v.OrganizationId <> @orgid
				AND om.MemberTypeId = 700 	-- vip
				AND @orgid = @bfclassid
	
	IF @GroupMeetingFlag = 1 AND @membertypeid IS NOT NULL	
	BEGIN
		IF EXISTS(SELECT NULL FROM @VIPAttendance)
			UPDATE dbo.Attend
			SET AttendanceTypeId = 20, -- volunteer
				MemberTypeId = @membertypeid
			WHERE AttendId = @AttendId
		ELSE
			UPDATE dbo.Attend			
			SET AttendanceTypeId = 90, -- group
				MemberTypeId = @membertypeid
			WHERE AttendId = @AttendId
	END
	ELSE IF @membertypeid IS NOT NULL and @membertypeid <> 311 -- is a member of this class and not a prospect
	BEGIN
		UPDATE dbo.Attend
		SET MemberTypeId = @membertypeid,
			AttendanceTypeId = dbo.GetAttendType(@Present, @membertypeid, @GroupMeetingFlag),
			BFCAttendance = (CASE WHEN @bfclassid = @orgid THEN 1 ELSE 0 END)
		WHERE AttendId = @AttendId
		
		IF @bfcid IS NOT NULL AND (@Present = 1 OR @bfcattend IS NOT NULL)
		BEGIN
            /* At this point I am recording attendance for a vip class                     
             * or for a class where I am doing InService (long term) teaching
             * And now I am looking at the BFClass where I am a regular member or an InService Member
             * I don't need to be here if I am reversing my attendance and there is no BFCAttendance to fix
             */
			IF @bfcattend IS NULL
				EXECUTE @bfcattend = dbo.CreateOtherAttend @MeetingId, @bfcid, @PeopleId
				
			UPDATE dbo.Attend
			SET OtherAttends = @Present
			WHERE AttendId = @bfcattend
			
			UPDATE dbo.Attend
			SET OtherAttends = ISNULL((SELECT AttendanceFlag FROM dbo.Attend WHERE AttendId = @bfcattend), 0)
			WHERE AttendId = @AttendId
			
			DECLARE @BFCOtherAttends BIT
			IF EXISTS(SELECT NULL FROM dbo.Attend WHERE AttendId = @bfcattend AND OtherAttends > 0)
				SET @BFCOtherAttends = 1
							
			IF @membertypeid = 700 -- VIP
				IF @BFCOtherAttends = 1
					UPDATE dbo.Attend
					SET AttendanceTypeId = 20 -- Volunteer
					WHERE AttendId = @bfcattend
				ELSE
					UPDATE dbo.Attend
					SET AttendanceTypeId = 30 -- Member
					WHERE AttendId = @bfcattend
			ELSE IF @membertypeid = 500 -- InService Member
				IF @BFCOtherAttends = 1
					UPDATE dbo.Attend
					SET AttendanceTypeId = 70 -- InService
					WHERE AttendId = @bfcattend
				ELSE					
					UPDATE dbo.Attend
					SET AttendanceTypeId = 30 -- Member
					WHERE AttendId = @bfcattend
					
			INSERT @OtherMeetings ( id ) VALUES  ( @bfcattend )
		END
		ELSE IF EXISTS(SELECT NULL FROM @VIPAttendance) -- need to indicate BFCAttendance or not
		BEGIN
		/* At this point I am recording attendance for a BFClass
         * And now I am looking at the one or more VIP classes where I a sometimes volunteer
         */
			UPDATE dbo.Attend
			SET OtherAttends = @Present
			WHERE AttendId IN (SELECT Id FROM @VIPAttendance)
			
			UPDATE dbo.Attend
			SET AttendanceTypeId = 20,-- Volunteer
				OtherAttends = 1
			WHERE AttendId = @AttendId
			
			INSERT @OtherMeetings (id) SELECT Id FROM @VIPAttendance
		END
	END
	ELSE -- not a member of this class
	BEGIN
		IF NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers
				WHERE OrganizationId = @bfcid
				AND PeopleId = @PeopleId)
		BEGIN
			IF EXISTS(SELECT NULL FROM Attend -- RecentVisitor?
					WHERE PeopleId = @PeopleId
					AND AttendanceFlag = 1
					AND MeetingDate >= @dt
					AND MeetingDate <= @meetdt
					AND OrganizationId = @orgid
					AND AttendanceTypeId IN (50, 60)) -- new and recent
				UPDATE dbo.Attend
				SET AttendanceTypeId = 50, -- RecentVisitor
					MemberTypeId = isnull(@membertypeid, 310) -- prospect or Visitor
				WHERE AttendId = @AttendId
			ELSE
				UPDATE dbo.Attend
				SET AttendanceTypeId = 60, -- NewVisitor
					MemberTypeId = isnull(@membertypeid, 310) -- prospect or Visitor
				WHERE AttendId = @AttendId
		END
		ELSE -- member of another class (visiting member)
		BEGIN
			IF @Present = 1
				UPDATE dbo.Attend
				SET AttendanceTypeId = 40, -- Visiting Member
					MemberTypeId = 310 -- Visiting Member
				WHERE AttendId = @AttendId
				
			IF @bfcattend IS NULL
				EXECUTE @bfcattend = dbo.CreateOtherAttend @MeetingId, @bfcid, @PeopleId
				
			UPDATE dbo.Attend
			SET OtherAttends = @Present
			WHERE AttendId = @bfcattend
			
			IF @Present = 1
				UPDATE dbo.Attend
				SET AttendanceTypeId = 110 -- Other Class
				WHERE AttendId = @bfcattend
			ELSE
			BEGIN
				DECLARE @mt INT, @bfcoid INT, @group BIT
				SELECT @bfcoid = om.OrganizationId, @mt = om.MemberTypeId, @group = m.GroupMeetingFlag
				FROM dbo.Attend a 
				JOIN dbo.OrganizationMembers om ON a.PeopleId = om.PeopleId AND a.OrganizationId = om.OrganizationId
				JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
				WHERE a.AttendId = @bfcattend
				
				UPDATE dbo.Attend
				SET AttendanceTypeId = dbo.GetAttendType(AttendanceFlag, @mt, @group)
				WHERE AttendId = @bfcattend
			END	
			INSERT @OtherMeetings (id) VALUES(@bfcattend)
		END
	END
	EXEC dbo.UpdateAttendStr @orgid, @PeopleId
	EXEC dbo.UpdateMeetingCounters @MeetingId
	EXEC dbo.AttendUpdateN @PeopleId, 10
	
	DECLARE othermeetingscursor CURSOR FOR SELECT DISTINCT id FROM @OtherMeetings
	OPEN othermeetingscursor
	DECLARE @oaid INT
	FETCH NEXT FROM othermeetingscursor INTO @oaid
	WHILE @@Fetch_Status=0
	BEGIN
		DECLARE @oid INT, @mid INT
		SELECT @oid = OrganizationId, @mid = MeetingId FROM dbo.Attend WHERE AttendId = @oaid
		EXEC dbo.UpdateAttendStr @oid, @PeopleId
		EXEC dbo.UpdateMeetingCounters @mid
		FETCH NEXT FROM othermeetingscursor INTO @oaid
	END
	CLOSE othermeetingscursor
	DEALLOCATE othermeetingscursor
	
	--COMMIT TRANSACTION
	SELECT 'ok' AS ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
