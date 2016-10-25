-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE TRIGGER [dbo].[delOrganizationMember] 
   ON  [dbo].[OrganizationMembers]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
		@skipInsertTriggerProcessing BIT

	SELECT
		@skipInsertTriggerProcessing = SkipInsertTriggerProcessing
	FROM Inserted

	IF @skipInsertTriggerProcessing = 1
		RETURN

	UPDATE dbo.Organizations
	SET MemberCount = dbo.OrganizationMemberCount(OrganizationId),
		ProspectCount = dbo.OrganizationProspectCount(OrganizationId)
	WHERE OrganizationId IN 
	(SELECT OrganizationId FROM DELETED GROUP BY OrganizationId)

	UPDATE dbo.People
	SET Grade = ISNULL(dbo.SchoolGrade(PeopleId), Grade),
	BibleFellowshipClassId = dbo.BibleFellowshipClassId(PeopleId)
	WHERE PeopleId IN (SELECT PeopleId FROM DELETED)

	DECLARE c CURSOR FOR
	SELECT d.OrganizationId, MemberTypeId, o.LeaderMemberTypeId FROM DELETED d
	JOIN dbo.Organizations o ON o.OrganizationId = d.OrganizationId
	OPEN c;
	DECLARE @oid INT, @mt INT, @lmt INT
	FETCH NEXT FROM c INTO @oid, @mt, @lmt;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF (@mt = @lmt)
		BEGIN
			UPDATE dbo.Organizations
			SET LeaderId = dbo.OrganizationLeaderId(OrganizationId),
			LeaderName = dbo.OrganizationLeaderName(OrganizationId)
			WHERE OrganizationId = @oid
			
			UPDATE dbo.People
			SET BibleFellowshipClassId = dbo.BibleFellowshipClassId(p.PeopleId)
			FROM dbo.People p
			JOIN dbo.OrganizationMembers om ON p.PeopleId = om.PeopleId
			WHERE om.OrganizationId = @oid
		END
		FETCH NEXT FROM c INTO @oid, @mt, @lmt;
	END;
	CLOSE c;
	DEALLOCATE c;

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
