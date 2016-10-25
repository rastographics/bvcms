CREATE PROCEDURE [dbo].[CreateMeeting]
    (
      @oid INT,
      @mdt DATETIME
    )
AS
BEGIN
	DECLARE @mid INT
	
	SELECT TOP 1 @mid = MeetingId
	FROM dbo.Meetings
	WHERE OrganizationId = @oid
	AND MeetingDate = @mdt
	
	IF (@mid IS NOT NULL)
		RETURN @mid

	BEGIN TRY
		DECLARE @acrid INT
		SELECT @acrid = AttendCreditId
		FROM dbo.OrgSchedule
		WHERE OrganizationId = @oid
		AND CAST(SchedTime AS TIME) = CAST(@mdt AS TIME)
		AND SchedDay = (DATEPART(dw, @mdt) - 1)

		INSERT dbo.Meetings
		        ( CreatedBy, CreatedDate , OrganizationId, MeetingDate, GroupMeetingFlag, AttendCreditId )
		VALUES  ( 0, GETDATE(), @oid, @mdt, 0, @acrid )
		RETURN SCOPE_IDENTITY()
	END TRY
	BEGIN CATCH
		SELECT TOP 1 @mid = MeetingId
		FROM dbo.Meetings
		WHERE OrganizationId = @oid
		AND MeetingDate = @mdt
		RETURN @mid
	END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
