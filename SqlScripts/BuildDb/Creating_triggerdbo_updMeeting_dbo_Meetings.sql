

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE TRIGGER [dbo].[updMeeting] 
   ON  [dbo].[Meetings]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mid INT, @grp BIT, @oid INT
	
	IF UPDATE(GroupMeetingFlag)
	BEGIN
		SELECT @mid = MeetingId, @grp = GroupMeetingFlag, @oid = OrganizationId FROM INSERTED
		
		IF (@grp <> 1)
			UPDATE dbo.Attend
			SET AttendanceTypeId = mt.AttendanceTypeId
			FROM dbo.Attend a
			JOIN dbo.OrganizationMembers m ON a.PeopleId = m.PeopleId AND a.OrganizationId = m.OrganizationId
			JOIN lookup.MemberType mt ON m.MemberTypeId = mt.Id
			WHERE a.MeetingId = @mid AND a.AttendanceTypeId = 90 AND a.AttendanceFlag = 0
		ELSE
			UPDATE dbo.Attend
			SET AttendanceTypeId = 90
			WHERE MeetingId = @mid AND AttendanceFlag = 0
			
		DECLARE @dialog UNIQUEIDENTIFIER
		BEGIN DIALOG CONVERSATION @dialog
		  FROM SERVICE UpdateAttendStrService
		  TO SERVICE 'UpdateAttendStrService'
		  ON CONTRACT UpdateAttendStrContract
		  WITH ENCRYPTION = OFF;
		SEND ON CONVERSATION @dialog MESSAGE TYPE UpdateAttendStrMessage (@oid)
	END
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
