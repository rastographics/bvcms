CREATE PROCEDURE [dbo].[CreateOtherAttend]
    (
      @mid INT,
      @oid INT,
      @pid INT
    )
AS
BEGIN
	DECLARE @mdt DATETIME
	
	SELECT @mdt = MeetingDate 
	FROM dbo.Meetings
	WHERE MeetingId = @mid
	
	DECLARE @notweekly BIT
	SELECT @notweekly = NotWeekly
	FROM dbo.Organizations
	WHERE OrganizationId = @oid

	DECLARE @omid INT
	SELECT TOP 1 @omid = MeetingId
	FROM dbo.Meetings
	WHERE OrganizationId = @oid
	AND MeetingDate = @mdt
	
	IF @omid IS NULL AND @notweekly = 1
		RETURN NULL

	EXEC @omid = dbo.CreateMeeting @oid, @mdt
	
	DECLARE @oaid INT
	SELECT @oaid = AttendId
	FROM dbo.Attend
	WHERE PeopleId = @pid
	AND MeetingId = @omid
	
	IF @oaid IS NULL
	BEGIN
		DECLARE @omt INT
		SELECT @omt = MemberTypeId
		FROM dbo.OrganizationMembers
		WHERE OrganizationId = @oid
		AND PeopleId = @pid
	
		INSERT dbo.Attend
		        ( MeetingId, PeopleId, OrganizationId, MeetingDate, CreatedDate, MemberTypeId )
		VALUES  ( @omid, @pid, @oid, @mdt, GETDATE(), @omt )
		SELECT @oaid = SCOPE_IDENTITY()
	END
	RETURN @oaid
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
