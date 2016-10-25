-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[updOrg] 
   ON  [dbo].[Organizations] 
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

/*
	IF UPDATE(SchedDay) OR UPDATE(SchedTime)
		UPDATE dbo.Organizations
		SET ScheduleId = dbo.ScheduleId(SchedDay, SchedTime),
		MeetingTime = dbo.GetScheduleTime(SchedDay, SchedTime)
		WHERE OrganizationId IN (SELECT OrganizationId FROM INSERTED)*/
	
	IF UPDATE(IsBibleFellowshipOrg)
		OR UPDATE(LeaderMemberTypeId)
	BEGIN
		IF UPDATE(LeaderMemberTypeId)
			UPDATE dbo.Organizations
			SET LeaderId = dbo.OrganizationLeaderId(OrganizationId),
			LeaderName = dbo.OrganizationLeaderName(OrganizationId)
			WHERE OrganizationId IN (SELECT OrganizationId FROM INSERTED)
		UPDATE dbo.People
		SET BibleFellowshipClassId = dbo.BibleFellowshipClassId(p.PeopleId)
		FROM dbo.People p
		JOIN dbo.OrganizationMembers m ON p.PeopleId = m.PeopleId
		JOIN INSERTED o ON m.OrganizationId = o.OrganizationId
	END
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
