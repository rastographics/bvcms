
CREATE PROCEDURE [dbo].[AddAbsents](@meetid INT, @userid INT)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @oid INT, @meetdate DATETIME, @attendType INT, @offsiteType INT, @groupflag BIT, @dt DATETIME
	SELECT @oid = OrganizationId, @meetdate = MeetingDate, 
		@attendType = 30, @offsiteType = 80, 
		@groupflag = GroupMeetingFlag, @dt = GETDATE()
	FROM dbo.Meetings m
	WHERE MeetingId = @meetid

	IF @groupflag <> 0
		RETURN

BEGIN TRY
	BEGIN TRANSACTION
	INSERT dbo.Attend 
	(
		MeetingId,
		OrganizationId,
		MeetingDate,
		CreatedBy,
		CreatedDate,
		OtherOrgId,
		PeopleId,
		MemberTypeId,
		AttendanceFlag,
		AttendanceTypeId
	)
	SELECT
		@meetid, 
		@oid, 
		@meetdate, 
		@userid,
		@dt, 
		NULL, 
		o.PeopleId, 
		o.MemberTypeId,
		0,
		@attendType
	FROM dbo.OrganizationMembers o
	WHERE o.OrganizationId = @oid
		AND NOT EXISTS
		(
			SELECT NULL FROM dbo.Attend a 
			WHERE a.PeopleId = o.PeopleId AND a.MeetingId = @meetid
		)

	UPDATE dbo.Attend
	SET 
		AttendanceFlag = NULL, 
		AttendanceTypeId = @offsiteType
	FROM dbo.OrganizationMembers o
	JOIN dbo.Attend a ON o.OrganizationId = a.OrganizationId
	WHERE a.MeetingId = @meetid AND a.PeopleId = o.PeopleId
	AND EXISTS
	(
		SELECT NULL FROM dbo.OrganizationMembers om2
		JOIN dbo.Organizations o2 ON om2.OrganizationId = o2.OrganizationId
		WHERE o2.Offsite = 1 AND om2.PeopleId = o.PeopleId
		AND o2.FirstMeetingDate <= @meetdate AND @meetdate <= DATEADD(dd, 1, o2.LastMeetingDate)
	) 
	COMMIT
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK
	DECLARE @em nvarchar(4000), @es INT
	SELECT @em=ERROR_MESSAGE(), @es=ERROR_SEVERITY()
	RAISERROR(@em, @es, 1)
END CATCH


END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
